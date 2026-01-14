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
        Assert.AreEqual(result, "0");
    }
}