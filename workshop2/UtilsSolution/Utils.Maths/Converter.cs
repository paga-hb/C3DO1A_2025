using System;

namespace Utils.Maths;

public class Converter : IConverter
{
    public double Convert(decimal value)
    {
        return decimal.ToDouble(value);
    }

    public decimal Convert(double value)
    {
        return (decimal)value;
    }
}