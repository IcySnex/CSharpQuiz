using CSharpQuiz.Helpers;
using CSharpQuiz.ViewModels;
using CSharpQuiz.Views;
using Microsoft.Extensions.Logging;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace CSharpQuiz.Services;

internal class AppStartupHandler
{
    public AppStartupHandler(
        ILogger<QuizViewModel> logger,
        ShellView mainWindow,
        SettingsViewModel settings,
        ContentDialogService dialogService,
        Navigation navigation)
    {
        settings.SetAccentColor(Color.FromArgb(255, 181, 141, 240));

        dialogService.SetContentPresenter(mainWindow.DialogPresenter);

        mainWindow.Show();
        mainWindow.TitleBar.Icon = new ImageIcon() { Source = Elements.IconImage };

        navigation.Navigate("Startseite");
        navigation.SetPaneOpen(false);

        logger.LogInformation("App wurde vollsändig gestartet.");
    }
}