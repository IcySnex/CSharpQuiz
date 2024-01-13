using CSharpQuiz.Helpers;
using CSharpQuiz.Services;
using CSharpQuiz.ViewModels;
using CSharpQuiz.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows; 
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace CSharpQuiz;

public partial class App : Application
{
    readonly IHost host;

    public static IServiceProvider Provider { get; private set; } = default!;
    public static InMemorySink Sink { get; } = new();
    public static Window? LoggerWindow { get; set; }

    public App()
    {
        host = Host.CreateDefaultBuilder()
            .UseSerilog((context, configuration) =>
            {
                configuration.WriteTo.Debug();
                configuration.WriteTo.Sink(Sink);
            })
            .ConfigureServices((context, services) =>
            {
                // Services
                services.AddSingleton<ContentDialogService>();
                services.AddSingleton<Navigation>();

                // ViewModels
                services.AddSingleton<HomeViewModel>();
                services.AddSingleton<SettingsViewModel>();

                // Main Window
                services.AddSingleton<ShellView>();
            })
            .Build();

        Provider = host.Services;
        InitializeComponent();
    }


    protected override void OnStartup(StartupEventArgs _)
    {
        ShellView mainWindow = Provider.GetRequiredService<ShellView>();
        SettingsViewModel settings = Provider.GetRequiredService<SettingsViewModel>();
        ContentDialogService dialogService = Provider.GetRequiredService<ContentDialogService>();
        Navigation navigation = Provider.GetRequiredService<Navigation>();

        settings.SetAccentColor(Color.FromArgb(255, 181, 141, 240));
        dialogService.SetContentPresenter(mainWindow.DialogPresenter);
        mainWindow.Show();
        mainWindow.TitleBar.Icon = new ImageIcon() { Source = Elements.IconImage };
        navigation.Navigate("Home");
        navigation.SetPaneOpen(false);
    }
}