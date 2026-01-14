namespace CalculatorTests;

[TestClass]
public class CalculatorUnitTests
{
    private Calculator.Calculator _calculator;

    [TestInitialize]
    public void Initialize()
    {
        _calculator = new Calculator.Calculator();
    }

    [TestMethod]
    public void EmptyAdd_MustReturnZero()
    {
        var value = "";
        var result = _calculator.Add(value);
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void SingleNumber_ReturnsSameNumber()
    {
        var value = "20";
        var result = _calculator.Add(value);
        Assert.AreEqual("20", result);
    }

    [TestMethod]
    public void TwoNumbers_ReturnsSum()
    {
        var value = "1,5000";
        var result = _calculator.Add(value);
        Assert.AreEqual("5001", result);
    }

    [TestMethod]
    public void NegativeNumbers_AreSummed()
    {
        var value = "4,-3";
        var result = _calculator.Add(value);
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void MoreThanTwoNumbers_ThrowsException()
    {
        var value = "1,2,3";
        Assert.ThrowsException<ArgumentException>(() => _calculator.Add(value));
    }

    [TestMethod]
    public void MissingFirstNumber_TreatedAsZero()
    {
        var value = ",5";
        var result = _calculator.Add(value);
        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public void MissingSecondNumber_TreatedAsZero()
    {
        var value = "5,";
        var result = _calculator.Add(value);
        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public void OnlyDelimiter_ReturnsZero()
    {
        var value = ",";
        var result = _calculator.Add(value);
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void InvalidSecondNumber_TreatedAsZero()
    {
        var value = "8,asd";
        var result = _calculator.Add(value);
        Assert.AreEqual("8", result);
    }

    [TestMethod]
    public void InvalidFirstNumber_TreatedAsZero()
    {
        var value = "sdasd,15";
        var result = _calculator.Add(value);
        Assert.AreEqual("15", result);
    }
}
