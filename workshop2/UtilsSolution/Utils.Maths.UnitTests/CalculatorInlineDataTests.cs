using System;
using Moq;
using Xunit.Abstractions;

namespace Utils.Maths.UnitTests;

public class CalculatorInlineDataTests
{
    private readonly ITestOutputHelper _output;

    public CalculatorInlineDataTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory] // <---------------------- We have replaced the [Fact] attribute with the [Theory] attribute 
    [InlineData(1, 2, 3)] // <--------- and two [InlineData] attributes, each with three values
    [InlineData(2, 4, 6)] // <--------- that are sent in as input parameter to the Test Method ---------------\
    public void Add_TwoNumbers_ReturnsCorrectSum(decimal a, decimal b, decimal expected) // <-----------------/
    {
        // Arrage
        
        var mock = new Mock<IConverter>();
        mock.Setup(m => m.Convert(a)).Returns(1.0);
        mock.Setup(m => m.Convert(b)).Returns(2.0);
        mock.Setup(m => m.Convert(3.0)).Returns(expected);
        
        Calculator calculator = new Calculator(mock.Object);

        // Act
        decimal actual = calculator.Add(a, b);
        _output.WriteLine($"{a} + {b} = {actual}");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]// <---------------------- We have replaced the [Fact] attribute with the [Theory] attribute
    [InlineData(1, 2, -1)] // <--------- and two [InlineData] attributes, each with three values
    [InlineData(2, 4, -2)] // <--------- that are sent in as input parameter to the Test Method --------------\
    public void Subtract_TwoNumbers_ReturnsCorrectDifference(decimal a, decimal b, decimal expected) // <-----/
    {
        // Arrage

        var mock = new Mock<IConverter>();
        mock.Setup(m => m.Convert(a)).Returns(1.0);
        mock.Setup(m => m.Convert(b)).Returns(2.0);
        mock.Setup(m => m.Convert(-1.0)).Returns(expected);

        Calculator calculator = new Calculator(mock.Object);

        // Act
        decimal actual = calculator.Subtract(a, b);
        _output.WriteLine($"{a} - {b} = {actual}");

        // Assert
        Assert.Equal(expected, actual);
    }
}