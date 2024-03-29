﻿using ColorPicker;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Helpers;
using CSharpQuiz.Views;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace CSharpQuiz.ViewModels;

public partial class SettingsViewModel : ObservableObject
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

        logger.LogInformation("SettingsViewModel wurde initialisiert.");
    }


    public void SetAccentColor(
        Color color)
    {
        ApplicationAccentColorManager.Apply(color);
        ApplicationThemeManager.Apply(ApplicationThemeManager.GetAppTheme(), WindowBackdropType.None, false);

        mainWindow.TitleBar.Icon = new ImageIcon() { Source = Elements.IconImage };

        logger.LogInformation("Akkzent Farbe wurde zu '{color}' aktualisiert.", color);
    }

    [RelayCommand]
    async Task ChangeAccentColorAsync()
    {
        SquarePicker picker = new()
        {
            SelectedColor = (Color)Application.Current.Resources["SystemAccentColor"],
            Height = 170,
            Width = 170,
            Margin = new(12, 0, 0, 0)
        };

        if (await dialogService.ShowSimpleDialogAsync(new()
        {
            CloseButtonText = "Abrechen",
            Title = "Wähle eine Farbe aus",
            PrimaryButtonText = "Okay",
            Content = picker
        }) != ContentDialogResult.Primary)
            return;

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

        ApplicationThemeManager.Apply(requestedTheme, WindowBackdropType.None, false);

        Application.Current.Resources["CardBackgroundOverlayBrush"] = new SolidColorBrush(value ? Color.FromRgb(50, 50, 50) : Color.FromRgb(255, 255, 255));

        logger.LogInformation($"Dunkelmodus wurde zu '{value}' aktualisiert.");
    }


    [RelayCommand]
    void CreateLoggerWindow()
    {
        if (App.LoggerWindow is not null)
        {
            App.LoggerWindow.Activate();
            return;
        }

        System.Windows.Controls.TextBlock textBlock = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new(4)
        };

        App.LoggerWindow = new()
        {
            Title = "CSharp Quiz (Logger)",
            Width = 600,
            Height = 300,
            Content = new PassiveScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = textBlock
            }
        };

        void handler(object? s, string e) =>
            Application.Current.Dispatcher.BeginInvoke(() => textBlock.Text += e);

        App.Sink.OnNewLog += handler;
        App.LoggerWindow.Closed += (s, e) =>
        {
            App.Sink.OnNewLog -= handler;
            App.LoggerWindow = null;
        };

        App.LoggerWindow.Show();

        logger.LogInformation("[HomeViewModel-CreateLoggerWindow] Neues LoggerWindow wurde erstellt und log-handler wurden gehooked.");
    }
}