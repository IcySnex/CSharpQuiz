using CSharpQuiz.Questions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.MultiConverter;

internal class IsReorderQuestionCorrect : IMultiValueConverter
{
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        string item = (string)values[0];
        ReorderQuestion question = (ReorderQuestion)values[1];

        int selectedIndex = question.Items.IndexOf(item);
        int correctIndex = Array.IndexOf(question.CorrectItemsOrder, item);

        return selectedIndex == correctIndex;
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture) =>
        throw new NotImplementedException();
}