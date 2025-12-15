using System.Collections.ObjectModel;
using ElevatorApi.Api.Config;
using ElevatorApi.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace ElevatorApi.Tests.Services;

[Category("Unit")]
public class CarServiceTests
{
    private Mock<ICarRepository> Repository { get; set; }
    private Mock<IOptions<ElevatorSettings>> Settings { get; set; }
    private CarService Sut { get; set; }

    [SetUp]
    public void Setup()
    {
        Repository = new();
        Settings = new();
        Sut = new(Repository.Object, Settings.Object);
    }

    #region GetById

    [Test]
    public void GetById_CarNotFound_ReturnsNull()
    {
        Repository.Setup(s => s.GetById(1)).Returns(null as Car);
        Assert.That(Sut.GetById(1), Is.Null);
    }

    [Test]
    public void GetById_CarFound_ReturnsCar()
    {
        var car = new Car(1, 0, 0, 1);

        Repository.Setup(s => s.GetById(car.Id))
            .Returns(car);

        Assert.That(Sut.GetById(car.Id), Is.EqualTo(car));
    }

    #endregion

    #region AddStop

    [Test]
    public void AddStop_CarNotFound_ReturnsNull()
    {
        byte carId = 1;
        Repository.Setup(s => s.GetById(carId))
            .Returns(null as Car);

        Assert.Throws<CarNotFoundException>(() =>
            Sut.AddStop(carId, 1));
    }

    [Test]
    public void AddStop_CarFound_AddsStop()
    {
        var car = new Car(1, 0, -1, 10);
        Repository.Setup(s => s.GetById(car.Id))
            .Returns(car);

        var actual = Sut.AddStop(car.Id, 2);
        Assert.That(actual.Stops, Does.Contain(2));
    }

    #endregion

    #region MoveCar

    [Test]
    public void MoveCar_CarNotFound_ReturnsNull()
    {
        byte carId = 1;
        Repository.Setup(s => s.GetById(carId))
            .Returns(null as Car);

        Assert.Throws<CarNotFoundException>(() =>
            Sut.MoveCar(carId));
    }

    [Test]
    public void MoveCar_CarFound_MovesCar()
    {
        var car = new Car(1, 0, -1, 10);
        Repository.Setup(s => s.GetById(car.Id))
            .Returns(car);

        car.AddStop(2);

        var actual = Sut.MoveCar(car.Id);
        Assert.That(actual.CurrentFloor, Is.EqualTo(2));
    }

    #endregion

    #region CallCar

    [Test]
    public void CallCar_AllIdle_AssignsCar()
    {
        var car = Sut.CallCar(1);
        Assert.That(car.NextFloor, Is.EqualTo(1));
    }

    [Test]
    public void CallCar_BetweenAscendingFloors_AssignsAscendingCar()
    {
        var cars = SetupCars();
        var car = cars.First();

        car.AddStop(8);

        var assigned = Sut.CallCar(7);

        Assert.Multiple(() =>
        {
            Assert.That(assigned.NextFloor, Is.EqualTo(7));
            Assert.That(assigned.Stops, Is.EqualTo(new sbyte[] { 7, 8 }));
        });
    }

    [Test]
    public void CallCar_AboveAscendingFloors_AssignsAscendingCar()
    {
        var cars = SetupCars();
        var car = cars.First();

        car.AddStop(5);

        var assigned = Sut.CallCar(7);

        Assert.Multiple(() =>
        {
            Assert.That(assigned.NextFloor, Is.EqualTo(5));
            Assert.That(assigned.Stops, Is.EqualTo(new sbyte[] { 5, 7 }));
        });
    }

    [Test]
    public void CallCar_CarAtFloor_AssignsCar()
    {
        var cars = SetupCars();
        var expected = cars.First();

        expected.AddStop(1);
        expected.MoveNext();

        var actual = Sut.CallCar(1);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CallCar_NoneIdle_AssignsClosest()
    {
        var cars = SetupCars();
        var car = cars.First();

        car.AddStop(8);
        car.MoveNext();
        car.AddStop(9);

        var second = cars.Skip(1).First();
        second.AddStop(6);
        second.AddStop(5);

        var expected = cars.Last();
        expected.AddStop(1);

        var assigned = Sut.CallCar(7);

        Assert.Multiple(() =>
        {
            Assert.That(assigned.NextFloor, Is.EqualTo(1));
            Assert.That(assigned.Stops, Is.EqualTo(new sbyte[] { 1, 7 }));
        });
    }

    [Test]
    public void CallCar_DescendingNearest_AssignsDescendingCar()
    {
        var cars = SetupCars();
        var car = cars.First();

        car.AddStop(-2);

        var assigned = Sut.CallCar(-1);

        Assert.Multiple(() =>
        {
            Assert.That(assigned.NextFloor, Is.EqualTo(-1));
            Assert.That(assigned.Stops, Is.EqualTo(new sbyte[] { -1, -2 }));
        });
    }

    [Test]
    public void CallCar_IdleNearest_AssignsIdleCar()
    {
        var cars = SetupCars();
        var ascendingCar = cars.First();
        ascendingCar.AddStop(3);

        var descendingCar = cars.Skip(1).First();
        descendingCar.AddStop(-1);

        var expected = cars.Last();
        var actual = Sut.CallCar(1);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.NextFloor, Is.EqualTo(1));
            Assert.That(actual.Stops, Is.EqualTo(new sbyte[] { 1 }));
        });
    }


    [TestCase(-1)]
    [TestCase(6)]
    public void CallCar_InvalidFloorNumber_ThrowsExpected(sbyte floorNumber)
    {
        Settings.Setup(s => s.Value.MinFloor)
            .Returns(0);
        Settings.Setup(s => s.Value.MaxFloor)
            .Returns(5);
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            Sut.CallCar(floorNumber));

        Assert.Multiple(() =>
        {
            Assert.That(ex.ParamName, Is.EqualTo("floorNumber"));
            Assert.That(ex.Message, Does.StartWith("floorNumber must be between 0 and 5"));
        });
    }

    #endregion

    /// <summary>
    /// Creates a list of test cars and sets up
    /// repository to return them, also sets up ElevatorSettings
    /// </summary>
    private List<Car> SetupCars()
    {
        var cars = Enumerable.Range(1, 3).Select(i =>
                new Car(
                    (byte)i,
                    Settings.Object))
            .ToList();

        Repository.Setup(s => s.GetAll())
            .Returns(cars);

        return cars;
    }
}