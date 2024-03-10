using System.Windows;
using System.Windows.Controls;

namespace CSharpQuiz.Helpers;

public partial class QuestionHeader : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(QuestionHeader));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }


    public static readonly DependencyProperty PointsProperty =
        DependencyProperty.Register("Points", typeof(int), typeof(QuestionHeader));

    public int Points
    {
        get => (int)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }


    public static readonly DependencyProperty TypeNoteProperty =
        DependencyProperty.Register("TypeNote", typeof(string), typeof(QuestionHeader));

    public string TypeNote
    {
        get => (string)GetValue(TypeNoteProperty);
        set => SetValue(TypeNoteProperty, value);
    }


    public QuestionHeader()
    {
        InitializeComponent();
    }
}