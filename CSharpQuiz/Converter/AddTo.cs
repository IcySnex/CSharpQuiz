using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.Converter;

public class AddTo : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        double input = (double)value;
        double toAdd = System.Convert.ToDouble(parameter);

        return input + toAdd;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        double input = (double)value;
        double toRemove = (double)parameter;

        return input - toRemove;
    }
}