using CSharpQuiz.ViewModels;
using System.Windows.Controls;

namespace CSharpQuiz.Views.Questions;

public partial class QuestionsView : UserControl
{
    public QuestionsView(
        QuizViewModel viewModel)
    {
        DataContext = viewModel;

        InitializeComponent();
    }
}