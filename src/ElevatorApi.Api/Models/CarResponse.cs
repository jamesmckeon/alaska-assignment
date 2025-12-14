namespace ElevatorApi.Api.Models;

public record CarResponse(
    int Id,
    sbyte? NextFloor,
    sbyte CurrentFloor,
    IReadOnlyCollection<sbyte> Stops)
{
};