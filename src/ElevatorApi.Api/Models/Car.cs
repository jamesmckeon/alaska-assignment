using Microsoft.VisualBasic;

namespace ElevatorApi.Models;

public sealed class Car: IEquatable<Car>
{
    public byte Id { get; init; }
    public IReadOnlyCollection<Floor> Destinations { get; } = new List<Floor>();
    public Floor? NextFloor { get; private set; }
    public bool? Ascending  { get; private set; }

    public void AddDestination(Floor destination)
    {
        throw new NotImplementedException();
    }

    public void Move()
    {
        throw new NotImplementedException();
    }

    public bool Equals(Car? other)
    {
        return  other != null && Id == other.Id;

    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Car);
    }
}