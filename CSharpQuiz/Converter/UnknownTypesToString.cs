using CSharpQuiz.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.Converter;

internal class UnknownTypesToString : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        UnknownTypes.ToString(value);

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        throw new NotImplementedException();
}