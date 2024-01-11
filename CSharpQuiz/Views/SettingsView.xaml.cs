using CSharpQuiz.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CSharpQuiz.Views;

public partial class SettingsView : Page
{
    public SettingsView()
    {
        DataContext = App.Provider.GetRequiredService<SettingsViewModel>();

        InitializeComponent();
    }
}