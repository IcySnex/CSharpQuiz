﻿using CSharpQuiz.Questions;
using System.Windows;
using System.Windows.Controls;

namespace CSharpQuiz.TemplateSelectors;

class QuestionResultTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(
        object item,
        DependencyObject container)
    {
        FrameworkElement element = (FrameworkElement)container;

        if (element is null || item is null || item is not Question question)
            return null;

        switch (question)
        {
            case SingleChoiceQuestion:
                return (DataTemplate)element.FindResource("SingleChoiceAnswerTemplate");
            case MultipleChoiceQuestion:
                return (DataTemplate)element.FindResource("MultipleChoiceAnswerTemplate");
            default:
                return null;
        }
    }

}