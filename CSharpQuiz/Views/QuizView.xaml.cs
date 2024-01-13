using CSharpQuiz.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CSharpQuiz.Views;

public partial class QuizView : Page
{
    public QuizView()
    {
        DataContext = App.Provider.GetRequiredService<QuizViewModel>();

        InitializeComponent();
    }
}