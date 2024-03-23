using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.Converter;

public class BooleanToOpacityConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        (bool)value ? 1 : 0.75;

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        (double)value == 1;
}