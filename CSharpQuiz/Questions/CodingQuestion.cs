using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Helpers;
using CSharpQuiz.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class CodingQuestion : Question
{
    readonly Random random = new();
    readonly ContentDialogService dialogService;
    readonly DynamicRuntime dynamicRuntime;

    readonly string defaultMethod;
    readonly object?[][]? argSets;
    readonly Delegate expectedDelegate;
    readonly string defaultTemplate;

    public bool IsCompiled => DynamicAssembly is not null;
    public string Solution { get; }

    public CodingQuestion(
        string text,
        string hint,
        double points,
        string defaultMethod,
        object?[][]? argSets,
        Delegate expectedDelegate,
        string defaultTemplate,
        string solution) : base(
            text,
            "Programmier-Frage: Vervollständige den unteren Programmcode.",
            $"Programmier-Frage: +{points} Punkte, wenn die Aufgabe erfüllt ist.",
            hint,
            points)
    {
        this.defaultMethod = defaultMethod;
        this.argSets = argSets;
        this.expectedDelegate = expectedDelegate;
        this.defaultTemplate = defaultTemplate;

        dialogService = App.Provider.GetRequiredService<ContentDialogService>();
        dynamicRuntime = App.Provider.GetRequiredService<DynamicRuntime>();

        Code = defaultTemplate;
        Output = null;
        Solution = solution;
    }


    void NotifyException(Exception ex)
    {
        if (ex is CompilationFailedException compilationEx)
        {
            CultureInfo culture = new("de-DE");
            int failureCount = compilationEx.Failures.Count();

            OutputKind = "Fehler";
            Output = $"{failureCount} Problem{(failureCount == 1 ? "" : "e")} gefunden\n- {string.Join("\n- ", compilationEx.Failures.Select(failure => $"Zeile {failure.Location.GetLineSpan().StartLinePosition.Line + 1}: {failure.GetMessage(culture)}"))}";
            return;
        }

        OutputKind = "Fehler";
        Output = ex.Message;
    }


    [ObservableProperty]
    byte[]? dynamicAssembly;

    [ObservableProperty]
    object?[]? args;

    [ObservableProperty]
    string code;

    partial void OnCodeChanged(string value)
    {
        OutputKind = null;
        Output = null;
        DynamicAssembly = null;
    }


    [ObservableProperty]
    string? outputKind = null;

    [ObservableProperty]
    string? output;

    [ObservableProperty]
    bool? isCorrect = null;


    [RelayCommand]
    public async Task ResetAsync()
    {
        if (await dialogService.ShowSimpleDialogAsync(new()
        {
            Title = "Bist du dir sicher?",
            Content = "Wenn du fortfährst, dann wird dein gesamter Programmcode zurück gesetzt und du startest wieder mit dem Anfangs-Template.",
            CloseButtonText = "Abbrechen",
            PrimaryButtonText = "Fortfahren"
        }) != ContentDialogResult.Primary)
            return;

        Code = defaultTemplate;
        OutputKind = null;
        Output = null;
        DynamicAssembly = null;
    }

    [RelayCommand]
    public async Task RunAsync()
    {
        if (!IsCompiled)
        {
            OutputKind = "Output";
            Output = "Kompilieren gestartet...";

            try
            {
                DynamicAssembly = await Task.Run(() => dynamicRuntime.Compile(Code));

                Output = "Kompilieren erfolgreich beendet.";
            }
            catch (Exception ex)
            {
                NotifyException(ex);
                return;
            }
        }

        Args = argSets?[random.Next(argSets.Length - 1)];
        Execute();
    }


    public object? Execute()
    {
        OutputKind = "Output";
        Output = "Ausführen gestartet...";

        try
        {
            object result = dynamicRuntime.Execute<object>(DynamicAssembly!, defaultMethod, Args) ?? throw new("Ausgeführte Methode gibt keinen Wert zurück");

            Output = $"Ausführen erfolgreich beendet:\n- Gegebene Parameter: {UnknownTypes.ToString(Args)}\n- Ausgegebner Wert: {UnknownTypes.ToString(result)}.";
            return result;
        }
        catch (Exception ex)
        {
            NotifyException(ex);
            return null;
        }
    }


    public override double ReachedPoints
    {
        get
        {
            if (IsCorrect.HasValue)
                return IsCorrect.Value ? Points : 0;

            Args = argSets?[random.Next(argSets.Length - 1)];
            object? expectedResult = expectedDelegate.DynamicInvoke(Args);
            if (expectedResult is null)
                return Points; // FEHLER: ES SOLLTE NIE NULL SEIN, DESWEGEN GEBEN WIR EINFACH MA SO ALLE PUNKTE

            if (!IsCompiled)
            {
                OutputKind = "Output";
                Output = "Kompilieren gestartet...";

                try
                {
                    DynamicAssembly = dynamicRuntime.Compile(Code);

                    Output = "Kompilieren erfolgreich beendet.";
                }
                catch (Exception ex)
                {
                    NotifyException(ex);
                    IsCorrect = false;
                    return 0;
                }
            }

            object? result = Execute();
            if (result is null)
            {
                IsCorrect = false;
                return 0;
            }

            Output = $"Ausführen erfolgreich beendet:\n- Gegebene Parameter: {UnknownTypes.ToString(Args)}\n- Ausgegebner Wert: {UnknownTypes.ToString(result)}\n- Erwarteter Wert: {UnknownTypes.ToString(expectedResult)}.";
            IsCorrect = UnknownTypes.AreEqual(result, expectedResult);
            return IsCorrect.Value ? Points : 0;
        }
    }
}