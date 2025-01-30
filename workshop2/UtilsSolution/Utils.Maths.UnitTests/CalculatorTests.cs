using System;
using Moq;
using Xunit.Abstractions;

namespace Utils.Maths.UnitTests;

public class CalculatorTests
{
    private readonly ITestOutputHelper _output;

    public CalculatorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Add_TwoNumbers_ReturnsCorrectSum()
    {
        // Arrage
        decimal a = 1;
        decimal b = 2;
        decimal expected = 3;
        
        var mock = new Mock<IConverter>();                   // <--- Here we are creating a mock of the IConverter interface

        mock.Setup(m => m.Convert(a)).Returns(1.0);          // <--- Here we defining what the Convert() ...
        mock.Setup(m => m.Convert(b)).Returns(2.0);          // <--- ... method in IConverter should return ...
        mock.Setup(m => m.Convert(3.0)).Returns(expected);   // <--- ... when receiving specific parameters
        
        Calculator calculator = new Calculator(mock.Object); // <--- Here we pass in an IConverter mock Object as an argument to the SUT's constructor

        // Act
        decimal actual = calculator.Add(a, b);
        _output.WriteLine($"{a} + {b} = {actual}");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Subtract_TwoNumbers_ReturnsCorrectDifference()
    {
        // Arrage
        decimal a = 1;
        decimal b = 2;
        decimal expected = -1;

        var mock = new Mock<IConverter>();                   // <--- Here we are creating a mock of the IConverter interface

        mock.Setup(m => m.Convert(a)).Returns(1.0);          // <--- Here we defining what the Convert() ...
        mock.Setup(m => m.Convert(b)).Returns(2.0);          // <--- ... method in IConverter should return ...
        mock.Setup(m => m.Convert(-1.0)).Returns(expected);  // <--- ... when receiving specific parameters

        Calculator calculator = new Calculator(mock.Object); // <--- Here we pass in an IConverter mock Object as an argument to the SUT's constructor

        // Act
        decimal actual = calculator.Subtract(a, b);
        _output.WriteLine($"{a} - {b} = {actual}");

        // Assert
        Assert.Equal(expected, actual);
    }
}