using System;

namespace Utils.Maths;

public interface IConverter
{
    double Convert(decimal value);
    decimal Convert(double value);
}