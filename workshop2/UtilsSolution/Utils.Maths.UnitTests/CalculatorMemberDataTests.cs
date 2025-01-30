using System;
using Moq;
using Xunit.Abstractions;

namespace Utils.Maths.UnitTests;

public class CalculatorMemberDataTests
{
    private readonly ITestOutputHelper _output;

    public CalculatorMemberDataTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static IEnumerable<object[]> AdditionData => new List<object[]> // <--- A static method that returns an array of data
    {
        new object[] { 1, 2, 3 },
        new object[] { 2, 4, 6 }
    };

    public static IEnumerable<object[]> SubtractionData => new List<object[]> // <--- A static method that returns an array of data
    {
        new object[] { 1, 2, -1 },
        new object[] { 2, 4, -2 }
    };

    [Theory] // <------------------------------ We're still using the [Theory] attribute,
    [MemberData(nameof(AdditionData))] // <--- but have replaced the numerous [InlineData] attributes with one [MemberData] attribute
    public void Add_TwoNumbers_ReturnsCorrectSum(decimal a, decimal b, decimal expected)
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

    [Theory] // <-------------------------------- We're still using the [Theory] attribute,
    [MemberData(nameof(SubtractionData))] // <--- but have replaced the numerous [InlineData] attributes with one [MemberData] attribute
    public void Subtract_TwoNumbers_ReturnsCorrectDifference(decimal a, decimal b, decimal expected)
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