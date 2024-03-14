using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class CodingQuestion : Question
{
    readonly ContentDialogService dialogService;
    readonly string defaultTemplate;

    public CodingQuestion(
        string text,
        string hint,
        double points,
        string defaultTemplate) : base(
            text,
            "Programmier-Frage: Vervollständige den unteren Programmcode. Teste dein Programm, indem fu auf 'Kompilieren' drückst oder setze alles auf Anfang, indem du auf 'Zurücksetzen' drückst.",
            $"Programmier-Frage: +{points} Punkte, wenn die Aufgabe erfüllt ist.",
            hint,
            points)
    {
        this.defaultTemplate = defaultTemplate;

        dialogService = App.Provider.GetRequiredService<ContentDialogService>();

        Code = defaultTemplate;
        Output = null;
    }


    [ObservableProperty]
    string code;

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
        Output = null;
    }

    [RelayCommand]
    public async Task CompileAsync()
    {
        Output = "Kompilieren gestartet...";
        await Task.Delay(1000);
    }


    public override double ReachedPoints
    {
        get
        {
            return 0;
        }
    }
}