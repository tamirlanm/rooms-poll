package poll

import (
	"context"
	"sync"

	"github.com/google/uuid"
	pb "github.com/tamirlanm/rooms-poll/services/poll/gen/pollpb"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
)

type Service struct {
	pb.UnimplementedPollServiceServer

	mu    sync.RWMutex
	polls map[string]*pb.PollResponse
}

func NewService() *Service {
	return &Service{
		polls: make(map[string]*pb.PollResponse),
	}
}

func (s *Service) CreatePoll(
	_ context.Context,
	req *pb.CreatePollRequest,
) (*pb.PollResponse, error) {

	if req.GetRoomId() == "" {
		return nil, status.Error(
			codes.InvalidArgument,
			"room_id is required",
		)
	}

	if req.GetQuestion() == "" {
		return nil, status.Error(
			codes.InvalidArgument,
			"question is required",
		)
	}

	if len(req.GetOptions()) < 2 {
		return nil, status.Error(
			codes.InvalidArgument,
			"poll must have at least 2 options",
		)
	}

	poll := &pb.PollResponse{
		Id:       uuid.NewString(),
		Room:     req.GetRoomId(),
		Question: req.GetQuestion(),
		Closed:   false,
	}

	for _, optionText := range req.GetOptions() {
		poll.Options = append(
			poll.Options,
			&pb.PollOption{
				Id:   uuid.NewString(),
				Text: optionText,
			},
		)
	}

	s.mu.Lock()
	s.polls[poll.Id] = poll
	s.mu.Unlock()

	return poll, nil
}

func (s *Service) GetPoll(
	_ context.Context,
	req *pb.GetPollRequest,
) (*pb.PollResponse, error) {

	s.mu.RLock()
	poll, exists := s.polls[req.GetPollId()]
	s.mu.RUnlock()

	if !exists {
		return nil, status.Error(
			codes.NotFound,
			"poll not found",
		)
	}

	return poll, nil
}

func (s *Service) Vote(
	_ context.Context,
	req *pb.VoteRequest,
) (*pb.PollResponse, error) {

	s.mu.Lock()
	defer s.mu.Unlock()

	poll, exists := s.polls[req.GetPollId()]
	if !exists {
		return nil, status.Error(
			codes.NotFound,
			"poll not found",
		)
	}

	if poll.Closed {
		return nil, status.Error(
			codes.FailedPrecondition,
			"poll is closed",
		)
	}

	for _, option := range poll.Options {
		if option.Id == req.GetOptionId() {
			option.VoteCount++

			return poll, nil
		}
	}

	return nil, status.Error(
		codes.NotFound,
		"option not found",
	)
}
