using ElevatorApi.Api.Config;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace ElevatorApi.Api.Models;

public sealed class Car: IEquatable<Car>
{
    
    public byte Id { get; init; }
    public IReadOnlyCollection<sbyte> DestinationFloors { get; } = new List<sbyte>();
    public sbyte CurrentFloor { get; set; }
    public sbyte? NextFloor{ get; private set; }
    /// <summary>
    /// will be null if "idle", i.e., there aren't any destinations in car's queue
    /// </summary>
    public bool? Ascending  { get; private set; }

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

    public void MoveToNextFloor()
    {
        throw new NotImplementedException();
    }
    
    // cars need to be instantiated at floor 1 by default
    /* call car, car is on floor already
     * cars need to be instantiated with first floor (or lobby -- add lobby floor # to appsettings)
     * next floor = 1st floor
     * last floor = null
     * client sets car destination
     * 
     */
}