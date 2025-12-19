namespace ElevatorApi.Api.Exceptions;

public class FloorNotFoundException : Exception
{
    public FloorNotFoundException(sbyte floorNumber) : base($"Floor #{floorNumber} doesn't exist")
    {
    }

    public FloorNotFoundException(string message) : base(message)
    {
    }

    public FloorNotFoundException()
    {
    }

    public FloorNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}