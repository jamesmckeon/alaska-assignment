using ElevatorApi.Api.Config;
using ElevatorApi.Api.Dal;
using ElevatorApi.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace ElevatorApi.Api.Services;

public class CarService : ICarService
{
    private ICarRepository CarRepository { get; }
    private IOptions<ElevatorSettings> Settings { get; }

    public CarService(ICarRepository carRepository, IOptions<ElevatorSettings> settings)
    {
        CarRepository = carRepository ??
                        throw new ArgumentNullException(nameof(carRepository));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public Car? GetById(byte id)
    {
        return CarRepository.GetById(id);
    }

    public Car AddStop(byte carId, sbyte floorNumber)
    {
        var car = CarRepository.GetById(carId) ??
                  throw new CarNotFoundException(carId);
        car.AddStop(floorNumber);
        return car;
    }

    public Car MoveCar(byte carId)
    {
        var car = CarRepository.GetById(carId) ??
                  throw new CarNotFoundException(carId);
        car.MoveNext();
        return car;
    }

    public Car CallCar(sbyte floorNumber)
    {
        if (floorNumber < Settings.Value.MinFloor || floorNumber > Settings.Value.MaxFloor)
        {
            throw new ArgumentOutOfRangeException($"floorNumber must be between {Settings.Value.MinFloor} and " +
                                                  $"{Settings.Value.MaxFloor}");
        }

        var cars = CarRepository.GetAll();
        Car? car = cars.FirstOrDefault(c => 
            c.NextFloor == floorNumber || c.CurrentFloor == floorNumber);
        

        if (car == null)
        {
            throw new NotImplementedException();
        }
    }
}