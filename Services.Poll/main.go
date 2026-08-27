package main

import (
	"log"
	"net"

	pb "github.com/tamirlanm/rooms-poll/services/poll/gen/pollpb"
	"github.com/tamirlanm/rooms-poll/services/poll/internal/poll"
	"google.golang.org/grpc"
)

func main() {
	listener, err := net.Listen("tcp", ":50051")
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
	}

	grpcServer := grpc.NewServer()

	pb.RegisterPollServiceServer(
		grpcServer,
		poll.NewService(),
	)

	log.Println("Poll Service listening on :50051")

	if err := grpcServer.Serve(listener); err != nil {
		log.Fatalf("failed to serve: %v", err)
	}
}
