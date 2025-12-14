using System.Data;
using System.Runtime;
using ElevatorApi.Api;
using ElevatorApi.Api.Config;
using Microsoft.Extensions.Options;

namespace ElevatorApi.Tests.Dal;

[Category("Unit")]
public class CarRepositoryTests
{
    private Mock<IOptions<ElevatorSettings>> SettingsOptions { get; set; }
    private CarRepository Sut { get; set; }

    [SetUp]
    public void SetUp()
    {
        SettingsOptions = new();
        Sut = new CarRepository(SettingsOptions.Object);
    }

    #region GetAll

    [TestCase(1)]
    [TestCase(3)]
    public void GetAll_MultipleCars_ReturnsExpected(byte carCount)
    {
        SettingsOptions.Setup(x => x.Value)
            .Returns(new ElevatorSettings()
            {
                CarCount = carCount,
                LobbyFloor = 0,
                MinFloor = 0,
                MaxFloor = 10
            });

        var expected = Enumerable.Range(1, carCount)
            .Select(id => new Car(
                (byte)id, 0, 0, 10))
            .ToList().AsReadOnly();

        var actual = Sut.GetAll();

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    public void GetAll_MultipleCars_SetsDefaults(sbyte initialFloor)
    {
        SettingsOptions.Setup(x => x.Value)
            .Returns(new ElevatorSettings()
            {
                CarCount = 3,
                LobbyFloor = -1,
                MinFloor = -2,
                MaxFloor = 10
            });

        var actual = Sut.GetAll();
        Assert.That(actual.All(a => a.CurrentFloor == -1), Is.True);
    }

    #endregion

    #region GetById

    [Test]
    public void GetById_CarExists_ReturnsExpectedCar()
    {
        SettingsOptions.Setup(x => x.Value)
            .Returns(new ElevatorSettings()
            {
                CarCount = 3,
                LobbyFloor = 1,
                MinFloor = -1,
                MaxFloor = 10
            });

        var expected = new Car(2, 1, 0, 1);
        var actual = Sut.GetById(expected.Id);

        Assert.That(actual, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actual.CurrentFloor,
                Is.EqualTo(1));
        });
    }

    [Test]
    public void GetById_NonExistentCar_ReturnsNull()
    {
        SettingsOptions.Setup(x => x.Value)
            .Returns(new ElevatorSettings()
            {
                CarCount = 1,
                LobbyFloor = 1,
                MinFloor = -1,
                MaxFloor = 10
            });

        Assert.That(Sut.GetById(2), Is.Null);
    }

    [TestCase(-1)]
    [TestCase(3)]
    public void GetById_VariousLobbyFloors_SetsDefaults(sbyte lobbyFloor)
    {
        SettingsOptions.Setup(x => x.Value)
            .Returns(new ElevatorSettings()
            {
                CarCount = 1,
                LobbyFloor = lobbyFloor,
                MinFloor = -1,
                MaxFloor = 10
            });

        var actual = Sut.GetById(1);

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.CurrentFloor, Is.EqualTo(lobbyFloor));
    }

    #endregion
}