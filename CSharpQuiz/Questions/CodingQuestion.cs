using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    readonly ContentDialogService dialogService;
    readonly DynamicRuntime dynamicRuntime;

    readonly string defaultMethod;
    readonly string defaultTemplate;
    readonly object?[]? args;
    readonly object expectedResult;

    public CodingQuestion(
        string text,
        string hint,
        double points,
        string defaultMethod,
        object?[]? args,
        object expectedResult,
        string defaultTemplate) : base(
            text,
            "Programmier-Frage: Vervollständige den unteren Programmcode.",
            $"Programmier-Frage: +{points} Punkte, wenn die Aufgabe erfüllt ist.",
            hint,
            points)
    {
        this.defaultMethod = defaultMethod;
        this.args = args;
        this.expectedResult = expectedResult;
        this.defaultTemplate = defaultTemplate;

        dialogService = App.Provider.GetRequiredService<ContentDialogService>();
        dynamicRuntime = App.Provider.GetRequiredService<DynamicRuntime>();

        Code = defaultTemplate;
        Output = null;
    }


    public bool IsCompiled => DynamicAssembly is not null;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteCommand))]
    byte[]? dynamicAssembly;

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
    public async Task CompileAsync()
    {
        OutputKind = "Output";
        Output = "Kompilieren gestartet...";

        try
        {
            DynamicAssembly = await Task.Run(() => dynamicRuntime.Compile(Code));
            Output = "Kompilieren erfolgreich beendet.";
        }
        catch (CompilationFailedException ex)
        {
            CultureInfo culture = new("de-DE");
            int failureCount = ex.Failures.Count();
            OutputKind = "Fehler";
            Output = $"{failureCount} Problem{(failureCount == 1 ? "" : "e")} gefunden\n- {string.Join("\n- ", ex.Failures.Select(failure => $"Zeile {failure.Location.GetLineSpan().StartLinePosition.Line + 1}: {failure.GetMessage(culture)}"))}";
        }
        catch (Exception ex)
        {
            OutputKind = "Fehler";
            Output = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(IsCompiled))]
    public void Execute()
    {
        OutputKind = "Output";
        Output = "Ausführen gestartet...";

        try
        {
            object result = ExecuteDynamicAssembly();
            Output = $"Ausführen erfolgreich beendet - Ausgegebener Wert: {result}.";
        }
        catch (Exception ex)
        {
            OutputKind = "Fehler";
            Output = ex.Message;
        }
    }

    object ExecuteDynamicAssembly()
    {
        object? result = dynamicRuntime.Execute<object>(DynamicAssembly!, defaultMethod, args) ?? throw new("Ausgeführte Methode gibt keinen Wert zurück");

        Type expectedType = expectedResult.GetType();
        if (result?.GetType() != expectedType)
            throw new($"Ausgeführte Methode gibt nicht den erwarteten Typ hearus: {expectedType.Name}");

        return result;
    }


    public override double ReachedPoints
    {
        get
        {
            try
            {
                DynamicAssembly = dynamicRuntime.Compile(Code);
                object result = ExecuteDynamicAssembly();

                return result == expectedResult ? Points : 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}