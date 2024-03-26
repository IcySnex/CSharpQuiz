using CSharpQuiz.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CSharpQuiz.Views;

public partial class FeedbackView : Page
{
    public FeedbackView()
    {
        DataContext = App.Provider.GetRequiredService<FeedbackViewModel>();

        InitializeComponent();
    }
}