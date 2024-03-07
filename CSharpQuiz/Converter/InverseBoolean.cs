using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.Converter;

internal class InverseBoolean : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        !(bool)value;

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        !(bool)value;
}