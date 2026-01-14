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
        var value = "1,1000";
        var result = _calculator.Add(value);
        Assert.AreEqual("1001", result);
    }

    [TestMethod]
    public void NegativeNumber_ShouldThrowException_ContainingNegative()
    {
        var value = "4,-3";
        var ex = Assert.ThrowsException<ArgumentException>(() => _calculator.Add(value));
        StringAssert.Contains(ex.Message, "-3");
    }

    [TestMethod]
    public void MultipleNegativeNumbers_ShouldThrowException_ContainingAllNegatives()
    {
        var value = "-1,-2,-3";
        var ex = Assert.ThrowsException<ArgumentException>(() => _calculator.Add(value));
        StringAssert.Contains(ex.Message, "-1");
        StringAssert.Contains(ex.Message, "-2");
        StringAssert.Contains(ex.Message, "-3");
    }

    [TestMethod]
    public void MoreThanTwoNumbers_ShouldAddThemUpAll()
    {
        var value = "1,2,3";
        var result = _calculator.Add(value);
        Assert.AreEqual("6", result);
    }

    [TestMethod]
    public void MoreThanTwoNumbers_Sample2_ShouldAddThemUpAll()
    {
        var value = "1,2,3,4,5,6,7,8,9,10,11,12";
        var result = _calculator.Add(value);
        Assert.AreEqual("78", result);
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

    [TestMethod]
    public void NewLine_AsAlternativeDelimiter_WorksWithCommas()
    {
        var value = "1\n2,3";
        var result = _calculator.Add(value);
        Assert.AreEqual("6", result);
    }

    [TestMethod]
    public void NewLine_AsDelimiter_WorksWithCommas()
    {
        var value = "1\n2\n7";
        var result = _calculator.Add(value);
        Assert.AreEqual("10", result);
    }

    [TestMethod]
    public void NumbersGreaterThanThousand_AreIgnored()
    {
        var value = "2,1001,6";
        var result = _calculator.Add(value);
        Assert.AreEqual("8", result);
    }

    [TestMethod]
    public void Thousand_IsIncludedInSum()
    {
        var value = "1000,1";
        var result = _calculator.Add(value);
        Assert.AreEqual("1001", result);
    }

    [TestMethod]
    public void CustomSingleCharacterDelimiter_IsSupported()
    {
        var value = "//#\n2#5";
        var result = _calculator.Add(value);
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void CustomDelimiter_Comma_UsesSameRules()
    {
        var value = "//,\n2,ff,100";
        var result = _calculator.Add(value);
        Assert.AreEqual("102", result);
    }

    [TestMethod]
    public void CustomDelimiter_Semicolon_WithLargeAndInvalidValues()
    {
        var value = "//;\n1000;2;2000;abc;3";
        var result = _calculator.Add(value);
        Assert.AreEqual("1005", result);
    }

    [TestMethod]
    public void CustomDelimiter_Asterisk_WithNegative_ThrowsAndListsNegative()
    {
        var value = "//*\n1*2*-4*1001";
        var ex = Assert.ThrowsException<ArgumentException>(() => _calculator.Add(value));
        StringAssert.Contains(ex.Message, "-4");
    }

    [TestMethod]
    public void CustomDelimiter_AnyLength_IsSupported()
    {
        var value = "//[***]\n11***22***33";
        var result = _calculator.Add(value);
        Assert.AreEqual("66", result);
    }

    [TestMethod]
    public void CustomSingleCharDelimiter_WithNoNumbers_ReturnsZero()
    {
        var value = "//;\n";
        var result = _calculator.Add(value);
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void CustomDelimiter_AnyLength_ConsecutiveDelimiters_IgnoredAsZero()
    {
        var value = "//[***]\n1******2"; // tokens: "1", "", "2"
        var result = _calculator.Add(value);
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void CustomDelimiter_AnyLength_WithRegexSpecialChars_Works()
    {
        var value = "//[.*]\n1.*2.*3";
        var result = _calculator.Add(value);
        Assert.AreEqual("6", result);
    }

    [TestMethod]
    public void CustomDelimiter_AnyLength_WithNegatives_ThrowsAndListsAll()
    {
        var value = "//[***]\n-1***2***-3";
        var ex = Assert.ThrowsException<ArgumentException>(() => _calculator.Add(value));
        StringAssert.Contains(ex.Message, "-1");
        StringAssert.Contains(ex.Message, "-3");
    }

    [TestMethod]
    public void CustomDelimiter_MalformedHeader_MissingClosingBracket_TreatedAsNoNumbers()
    {
        var value = "//[***\n1***2***3"; // header not in valid format, everything becomes invalid tokens
        var result = _calculator.Add(value);
        Assert.AreEqual("0", result);
    }

    [TestMethod]
    public void CustomDelimiter_WithDefaultDelimiters_AllApply()
    {
        var value = "//;\n1;2,3\n4"; // custom ';' plus default ',' and '\n'
        var result = _calculator.Add(value);
        Assert.AreEqual("10", result);
    }
}
