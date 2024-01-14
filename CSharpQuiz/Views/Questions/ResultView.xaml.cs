using CSharpQuiz.ViewModels;
using System.Windows.Controls;

namespace CSharpQuiz.Views.Questions;

public partial class ResultView : UserControl
{
    public ResultView(
        QuizViewModel viewModel)
    {
        DataContext = viewModel;

        InitializeComponent();
    }
}