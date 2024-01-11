using ColorPicker;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Helpers;
using CSharpQuiz.Views;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace CSharpQuiz.ViewModels;

internal partial class SettingsViewModel : ObservableObject
{
    readonly ILogger<SettingsViewModel> logger;
    readonly ContentDialogService dialogService;
    readonly ShellView mainWindow;

    public SettingsViewModel(
        ILogger<SettingsViewModel> logger,
        ContentDialogService dialogService,
        ShellView mainWindow)
    {
        this.logger = logger;
        this.dialogService = dialogService;
        this.mainWindow = mainWindow;

        IsDarkMode = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark;
    }


    public void SetAccentColor(
        Color color)
    {
        ApplicationAccentColorManager.Apply(color);
        ApplicationThemeManager.Apply(ApplicationThemeManager.GetAppTheme(), updateAccent: false);

        mainWindow.TitleBar.Icon = new ImageIcon() { Source = Elements.IconImage };
    }

    [RelayCommand]
    async Task ChangeAccentColorAsync()
    {
        logger.LogInformation("lol");

        SquarePicker picker = new()
        {
            SelectedColor = (Color)Application.Current.Resources["SystemAccentColor"]
        };

        if (await dialogService.ShowSimpleDialogAsync(new()
            {
                CloseButtonText = "Abrechen",
                Title = "Wähle eine Farbe aus",
                PrimaryButtonText = "Okay",
                Content = picker
            }) == ContentDialogResult.Primary)
            SetAccentColor(picker.SelectedColor);
    }


    [ObservableProperty]
    bool isDarkMode;

    partial void OnIsDarkModeChanged(
        bool value)
    {
        ApplicationTheme requestedTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        if (requestedTheme == ApplicationThemeManager.GetAppTheme())
            return;

        ApplicationThemeManager.Apply(requestedTheme, updateAccent: false);
    }
}