using ElevatorApi.Api.Models;

namespace ElevatorApi.Api.Services;

public interface ICarService
{
    Car? GetById(int id);
}