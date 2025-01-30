using System;

namespace Utils.Maths;

public class Calculator
{
    private readonly IConverter _converter;

    public Calculator(IConverter converter)
    {
        _converter = converter;
    }

    public decimal Add(decimal a, decimal b)
    {
        double x = _converter.Convert(a);
        double y = _converter.Convert(b);
        double result = x + y;
        return _converter.Convert(result);
    }

    public decimal Subtract(decimal a, decimal b)
    {
        double x = _converter.Convert(a);
        double y = _converter.Convert(b);
        double result = x - y;
        return _converter.Convert(result);
    }
}