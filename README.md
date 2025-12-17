# Elevator Control System API

A RESTful elevator control system API for integration/E2E testing

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Quick Start

Clone the repository and run with a single command:

```bash
cd src/ElevatorApi.Api
dotnet run
```

The API will be available at `http://localhost:8080`

## Configuration

The elevator system can be configured in `src/ElevatorApi.Api/appsettings.json`:

```json
{
  "ElevatorSettings": {
    "MinFloor": -2,        // Minimum valid floor number
    "MaxFloor": 50,        // Maximum valid floor number
    "NumberOfCars": 3,      // Number of elevator cars in the system
    "LobbyFloor": 0.       // The start floor for all cars 
  }
}
```

## API Endpoints

- swagger ui is available at http://localhost:8080/swagger

### Get Car State

```bash
# Get current state of a specific car
curl http://localhost:8080/api/cars/1
```

**Response:**

```json
{
  "carId": 1,
  "currentFloor": 0,
  "direction": "idle",
  "nextFloor": null,
  "origins": [],
  "destinations": []
}
```

### Request Pickup

```bash
# Request an elevator to pick up at floor 5
curl -X POST "http://localhost:8080/cars/call/5"
```

### Add Destination

```bash
# Add destination floor 10 to car 1
curl -X POST http://localhost:8080/cars/1/stops/10
```

### Advance Car

```bash
# Move car 1 to its next floor
curl -X POST http://localhost:8080/cars/1/move
```

## Testing

Run tests:

```bash
cd src/ElevatorApi.Tests
dotnet test
```

Run by category:

```bash
  dotnet test --filter Category=Unit
  dotnet test --filter Category=Integration
```

### Car Assignment Rules

**When called floor is within the range of assigned stops for all cars:**

1. Assign car with fewest stops before reaching called floor
2. Tiebreaker: car whose nearest existing stop is closest to called floor
3. Rationale: minimizes wait time (door cycles > travel distance)

Example: Called floor = 1

- Car 1: stops [7,6,-1] → 2 stops before floor 1, nearest stop -1 (2 floors away)
- Car 2: stops [8,5,0] → 2 stops before floor 1, nearest stop 0 (1 floor away) ✓
- Car 3: stops [7,6,2,-2] → 3 stops before floor 1
- Result: Car 2 assigned (tied on stops, wins on proximity)

**Idle behavior:**

- Cars remain at current floor when idle
