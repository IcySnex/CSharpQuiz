using CSharpQuiz.ViewModels;
using System.Windows.Controls;

namespace CSharpQuiz.Views.Questions;

public partial class WelcomeView : UserControl
{
    public WelcomeView(
        QuizViewModel viewModel)
    {
        DataContext = viewModel;

        InitializeComponent();
    }
}