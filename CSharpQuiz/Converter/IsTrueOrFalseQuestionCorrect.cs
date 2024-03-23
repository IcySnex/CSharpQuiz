using CSharpQuiz.Questions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.Converter;

public class IsTrueOrFalseItemCorrect : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        TrueOrFalseItem item = (TrueOrFalseItem)value;

        return item.IsCorrect ? item.IsTrue : item.IsFalse;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        throw new NotImplementedException();
}