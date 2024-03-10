using System.Windows;
using System.Windows.Controls;

namespace CSharpQuiz.Helpers;

public partial class AnswerHeader : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(AnswerHeader));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }


    public static readonly DependencyProperty PointsProperty =
        DependencyProperty.Register("Points", typeof(int), typeof(AnswerHeader));

    public int Points
    {
        get => (int)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }


    public static readonly DependencyProperty ReachedPointsProperty =
            DependencyProperty.Register("ReachedPoints", typeof(int), typeof(AnswerHeader));

    public int ReachedPoints
    {
        get => (int)GetValue(ReachedPointsProperty);
        set => SetValue(ReachedPointsProperty, value);
    }


    public static readonly DependencyProperty TypeAnswerNoteProperty =
        DependencyProperty.Register("TypeAnswerNote", typeof(string), typeof(AnswerHeader));

    public string TypeAnswerNote
    {
        get => (string)GetValue(TypeAnswerNoteProperty);
        set => SetValue(TypeAnswerNoteProperty, value);
    }


    public AnswerHeader()
    {
        InitializeComponent();
    }
}