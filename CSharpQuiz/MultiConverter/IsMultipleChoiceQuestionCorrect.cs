using CSharpQuiz.Questions;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CSharpQuiz.MultiConverter;

internal class IsMultipleChoiceQuestionCorrect : IMultiValueConverter
{
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        string item = (string)values[0];
        MultipleChoiceQuestion question = (MultipleChoiceQuestion)values[1];

        return question.CorrectAnswers.Contains(item);
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture) =>
        throw new NotImplementedException();
}