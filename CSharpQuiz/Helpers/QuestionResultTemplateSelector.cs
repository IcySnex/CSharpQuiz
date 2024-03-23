using CSharpQuiz.Questions;
using System.Windows;
using System.Windows.Controls;

namespace CSharpQuiz.Helpers;

public class QuestionResultTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(
        object item,
        DependencyObject container)
    {
        FrameworkElement element = (FrameworkElement)container;

        if (element is null || item is null || item is not Question question)
            return null;

        return question switch
        {
            SingleChoiceQuestion => (DataTemplate)element.FindResource("SingleChoiceAnswerTemplate"),
            MultipleChoiceQuestion => (DataTemplate)element.FindResource("MultipleChoiceAnswerTemplate"),
            ReorderQuestion => (DataTemplate)element.FindResource("ReorderAnswerTemplate"),
            TrueOrFalseQuestion => (DataTemplate)element.FindResource("TrueOrFalseAnswerTemplate"),
            CodingQuestion => (DataTemplate)element.FindResource("CodingAnswerTemplate"),
            _ => null,
        };
    }

}