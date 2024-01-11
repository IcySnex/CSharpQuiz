using CSharpQuiz.Views;
using Microsoft.Extensions.Logging;

namespace CSharpQuiz.Services;

internal class Navigation
{
    readonly ILogger<Navigation> logger;
    readonly ShellView mainWindow;

    public Navigation(
        ILogger<Navigation> logger,
        ShellView mainWindow)
    {
        this.logger = logger;
        this.mainWindow = mainWindow;
    }


    public void SetPaneOpen(
        bool isOpen) =>
        mainWindow.Navigation.IsPaneOpen = isOpen;

    public bool Navigate(
        string page)
    {
        logger.LogInformation($"Navigating to page: {page}");

        return mainWindow.Navigation.Navigate(page);
    }
}