# PulseRooms

A backend project for creating rooms and running polls, built with ASP.NET Core, Go, gRPC, and PostgreSQL.

> **Project status:** Work in progress. Room data is stored in PostgreSQL, while poll data is currently stored in memory.

## Tech Stack

- **C# / ASP.NET Core 10** — REST API for room management
- **Go** — gRPC poll service
- **gRPC + Protocol Buffers** — communication between clients and the poll service
- **PostgreSQL 17** — persistent room storage
- **Entity Framework Core + Npgsql** — database access
- **Docker Compose** — local database environment
- **OpenAPI + Scalar** — API documentation during development

## Architecture

```text
                 ┌──────────────────────┐
                 │      API Clients     │
                 └──────────┬───────────┘
                            │ HTTP / REST
                 ┌──────────▼───────────┐
                 │  Services.Api (.NET) │
                 │     Room endpoints   │
                 └──────────┬───────────┘
                            │ EF Core
                 ┌──────────▼───────────┐
                 │      PostgreSQL      │
                 └──────────────────────┘


                 ┌──────────────────────┐
                 │     gRPC Clients     │
                 └──────────┬───────────┘
                            │ gRPC
                 ┌──────────▼───────────┐
                 │  Services.Poll (Go)  │
                 │ Create / Get / Vote  │
                 └──────────────────────┘
```

The application separates room management from poll operations into two services:

- **Services.Api** manages rooms through a REST API and persists data in PostgreSQL.
- **Services.Poll** provides poll creation, retrieval, and voting through gRPC.
- **Protocol Buffers** define the gRPC contract shared by clients and the poll service.

## Project Structure

```text
rooms-poll/
├── Services.Api/       # ASP.NET Core REST API
├── Services.Poll/      # Go gRPC poll service
├── proto/              # Protocol Buffers definitions
├── docker-compose.yml  # PostgreSQL configuration
└── PulseRooms.slnx     # .NET solution
```

## Getting Started

### Prerequisites

Make sure you have installed:

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Go](https://go.dev/dl/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Docker Engine with Docker Compose

### 1. Clone the repository

```bash
git clone https://github.com/tamirlanm/rooms-poll.git
cd rooms-poll
```

### 2. Start PostgreSQL

```bash
docker compose up -d postgres
```

The development Docker Compose configuration exposes PostgreSQL on port `5433` and creates the `pulse_rooms_db` database.

The room API uses the connection string configured in `Services.Api/appsettings.json`.

> **Note:** Database credentials in development configuration should not be reused in production. Use environment variables or a secrets manager for sensitive values.

### 3. Start the REST API

Open a terminal and run:

```bash
cd Services.Api
dotnet restore
dotnet run
```

The configured local endpoints are:

- HTTP: `http://localhost:5109`
- HTTPS: `https://localhost:7054`

OpenAPI and Scalar API reference endpoints are available in Development mode, as configured by the application.

Ensure the database schema has been created before using the room endpoints. Apply existing Entity Framework Core migrations, or create an initial migration if needed.

### 4. Start the gRPC poll service

Open another terminal:

```bash
cd Services.Poll
go mod download
go run .
```

The gRPC server listens on port `50051` by default.

Keep both services running while developing or testing the application.

## REST API

The room API currently provides the following endpoints:

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/rooms` | Create a room |
| `GET` | `/api/rooms/{id}` | Retrieve a room by ID |

### Create a room

```bash
curl -X POST "http://localhost:5109/api/rooms" \
  -H "Content-Type: application/json" \
  -d '{"name":"Study Room"}'
```

A successful request returns `201 Created` with a room representation similar to:

```json
{
  "id": "<room-guid>",
  "name": "Study Room",
  "createdAt": "<utc-timestamp>",
  "memberCount": 0
}
```

The API validates the room name and returns `404 Not Found` when the requested room does not exist.

### Get a room

Replace `<room-guid>` with the ID returned when creating the room.

```bash
curl "http://localhost:5109/api/rooms/<room-guid>"
```

## gRPC Poll Service

The gRPC service contract is defined in [`proto/poll.proto`](proto/poll.proto).

| RPC | Description |
|---|---|
| `CreatePoll` | Create a poll for a room with a question and at least two options |
| `GetPoll` | Retrieve a poll by ID |
| `Vote` | Submit a vote for an option in an open poll |

The poll service supports basic validation, poll retrieval, and voting. It reports missing polls or options and rejects votes when a poll is closed.

Each poll option has an ID, text, and vote count.

### Current limitations

- Poll data is stored in memory and is lost when the Go service restarts.
- Poll persistence is not yet implemented.
- Authentication and authorization are not implemented.
- The `user_id` field in vote requests is not currently used to enforce one vote per user.
- The project is still under development and may require additional setup before all functionality is available.

## Configuration

The room API reads its database connection string from:

```text
ConnectionStrings:DefaultConnection
```

For local development, the connection string targets PostgreSQL on port `5433`.

Before deploying the application, consider:

- Moving credentials into environment variables or a secrets manager.
- Configuring database migrations for deployment.
- Adding persistent storage for polls.
- Implementing authentication and authorization.
- Enforcing voting rules.
- Adding automated tests, health checks, logging, and deployment configuration.

## Future Improvements

Potential areas for further development include:

- Persistent poll storage.
- User authentication and authorization.
- Vote validation and duplicate-vote prevention.
- Automated unit and integration tests.
- Health checks and structured logging.
- Production deployment with container orchestration.

## Contributing

Contributions, bug reports, and feature suggestions are welcome. For significant changes, consider opening an issue before submitting a pull request.

## License

No license has been specified for this repository yet. All rights remain with the respective copyright holders until a license is added.
