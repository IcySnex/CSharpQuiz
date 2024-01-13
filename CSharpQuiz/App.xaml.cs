using CSharpQuiz.Helpers;
using CSharpQuiz.Services;
using CSharpQuiz.ViewModels;
using CSharpQuiz.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows;
using Wpf.Ui;

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
                services.AddSingleton<AppStartupHandler>();

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


    protected override void OnStartup(StartupEventArgs _) =>
        Provider.GetRequiredService<AppStartupHandler>();
}