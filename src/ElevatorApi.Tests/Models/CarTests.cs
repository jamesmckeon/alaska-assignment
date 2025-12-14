namespace ElevatorApi.Tests.Models;

[Category("Unit")]
public class CarTests
{
    #region Equals

    [Test]
    public void Equals_SameCarNumber_ReturnsTrue()
    {
        var firstCar = new Car(1, 0);
        var secondCar = new Car(1, 1);
        Assert.That(firstCar.Equals(secondCar), Is.True);
    }

    [Test]
    public void Equals_DifferentCarNumber_ReturnsFalse()
    {
        var firstCar = new Car(1, 0);
        var secondCar = new Car(2, 0);
        Assert.That(firstCar.Equals(secondCar), Is.False);
    }

    #endregion

    #region Equals GetHashCode

    [TestCase(1)]
    [TestCase(0)]
    public void GetHashcode_ReturnsExpected(byte id)
    {
        var car = new Car(id, 0);
        Assert.That(car.GetHashCode(), Is.EqualTo(id.GetHashCode()));
    }

    #endregion

    #region Equals (Object Overload)

    [Test]
    public void EqualsOverload_DifferentType_ReturnsFalse()
    {
        var tesObj = new
        {
            Id = (byte)1
        };

        var sut = new Car(1, 0);

        Assert.That(sut.Equals(tesObj), Is.False);
    }

    [Test]
    public void EqualsOverload_SameType_ReturnsTrue()
    {
        var firstCar = new Car(1, 0);
        var secondCar = new Car(1, 0);
        Assert.That(firstCar.Equals(secondCar as Object), Is.True);
    }

    [Test]
    public void EqualsOverload_SameType_ReturnsFalse()
    {
        var firstCar = new Car(1, 0);
        var secondCar = new Car(2, 0);
        Assert.That(firstCar.Equals(secondCar as Object), Is.False);
    }

    #endregion

    #region Move

    #endregion
}