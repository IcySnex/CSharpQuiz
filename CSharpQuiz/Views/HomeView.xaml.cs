using CSharpQuiz.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CSharpQuiz.Views;

public partial class HomeView : Page
{
    public HomeView()
    {
        DataContext = App.Provider.GetRequiredService<HomeViewModel>();

        InitializeComponent();
    }
}