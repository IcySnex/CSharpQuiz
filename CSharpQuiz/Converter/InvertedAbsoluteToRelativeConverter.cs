using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.Converter;

public class InvertedAbsoluteToRelativeConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        double absolute = (double)value;
        double relative = System.Convert.ToDouble(parameter);

        return 1 - (relative / absolute);
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        (double)value == 1;
}