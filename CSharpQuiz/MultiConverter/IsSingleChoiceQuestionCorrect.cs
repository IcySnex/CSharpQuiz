﻿using CSharpQuiz.Questions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CSharpQuiz.MultiConverter;

public class IsSingleChoiceQuestionCorrect : IMultiValueConverter
{
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        string item = (string)values[0];
        SingleChoiceQuestion question = (SingleChoiceQuestion)values[1];

        return item == question.CorrectAnswer;
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture) =>
        throw new NotImplementedException();
}