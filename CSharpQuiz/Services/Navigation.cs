using CSharpQuiz.Views;
using Microsoft.Extensions.Logging;

namespace CSharpQuiz.Services;

public class Navigation(
    ILogger<Navigation> logger,
    ShellView mainWindow)
{
    readonly ILogger<Navigation> logger = logger;
    readonly ShellView mainWindow = mainWindow;


    public void SetPaneOpen(
        bool isOpen) =>
        mainWindow.Navigation.IsPaneOpen = isOpen;

    public bool Navigate(
        string page)
    {
        logger.LogInformation("Navigiert zur '{page}' Seite", page);

        return mainWindow.Navigation.Navigate(page);
    }
}