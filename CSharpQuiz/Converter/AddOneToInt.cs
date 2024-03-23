using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.Converter;

public class AddOneToInt : IValueConverter
{
    public object? Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        value is int number ? number + 1 : null;

    public object? ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        value is int number ? number - 1 : null;
}