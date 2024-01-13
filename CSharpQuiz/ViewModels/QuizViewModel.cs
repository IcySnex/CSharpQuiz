using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Questions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace CSharpQuiz.ViewModels;

internal partial class QuizViewModel : ObservableObject
{
    readonly ILogger<QuizViewModel> logger;
    readonly ContentDialogService dialogService;

    public QuizViewModel(
        ILogger<QuizViewModel> logger,
        ContentDialogService dialogService)
    {
        this.logger = logger;
        this.dialogService = dialogService;;

        timeEvolvedTimer.Tick += (s, e) =>
        {
            TimeEvolved = stopwatch.Elapsed.ToString("mm\\:ss");
        };

        logger.LogInformation("QuizViewModel wurde initialisiert.");
    }


    [ObservableProperty]
    Question? currentQuestion = null;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoNextCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoBackCommand))]
    [NotifyPropertyChangedFor(nameof(IsLastQuestion))]
    int? currentQuestionIndex = null;

    List<Question> questions = new()
    {
        new SingleChoiceQuestion(
            text: "Test Question",
            hint: "Nothing at all lol",
            points: 3,
            correctIndex: 0,
            "First", "Second", "Third"),
        new SingleChoiceQuestion(
            text: "This is another question LOLLLL",
            hint: "Fortnite",
            points: 3,
            correctIndex: 0,
            "sddsdsds", "223534fgd", "fddfdf")
    };

    public bool CanGoNext =>
        CurrentQuestionIndex is not null && CurrentQuestionIndex < questions.Count - 1;

    public bool CanGoBack =>
        CurrentQuestionIndex is not null && CurrentQuestionIndex > 0;

    public bool IsLastQuestion =>
        CurrentQuestionIndex is not null && CurrentQuestionIndex == questions.Count - 1;


    [ObservableProperty]
    bool isQuizRunning = false;

    public int QuestionCount => questions.Count;

    [ObservableProperty]
    int? hintCount = null;

    [ObservableProperty]
    string? timeEvolved = null;


    readonly Stopwatch stopwatch = new();
    readonly DispatcherTimer timeEvolvedTimer = new() { Interval = TimeSpan.FromSeconds(1) };


    [RelayCommand(CanExecute = nameof(CanGoNext))]
    void GoNext()
    {
        if (CurrentQuestionIndex is null)
            return;

        CurrentQuestionIndex++;
        CurrentQuestion = questions[CurrentQuestionIndex.Value];

        logger.LogInformation($"Es wurde zur nächsten Frage gewechselt.");
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    void GoBack()
    {
        if (CurrentQuestionIndex is null)
            return;

        CurrentQuestionIndex--;
        CurrentQuestion = questions[CurrentQuestionIndex.Value];

        logger.LogInformation($"Es wurde zur letzten Frage gewechselt.");
    }


    [RelayCommand]
    void Start()
    {
        IsQuizRunning = true;
        HintCount = 0;
        TimeEvolved = "00:00";
        stopwatch.Start();
        timeEvolvedTimer.Start();

        CurrentQuestionIndex = 0;
        CurrentQuestion = questions[CurrentQuestionIndex.Value];

        logger.LogInformation($"Quiz wurde gestartet.");
    }

    [RelayCommand]
    async Task StopAsync()
    {
        if (!IsLastQuestion && await dialogService.ShowSimpleDialogAsync(new()
        {
            Title = "Bist du dir sicher?",
            Content = "Wenn du das Quiz jetzt beendest werden die restlichen Fragen als falsch makiert.",
            CloseButtonText = "Abbrechen",
            PrimaryButtonText = "Fortfahren"
        }) != ContentDialogResult.Primary)
            return;

        IsQuizRunning = false;
        HintCount = null;
        TimeEvolved = null;
        stopwatch.Stop();
        stopwatch.Reset();
        timeEvolvedTimer.Stop();

        CurrentQuestionIndex = null;
        CurrentQuestion = null;

        logger.LogInformation($"Quiz wurde beendet.");

        EvaluateResult();
    }


    [RelayCommand]
    async Task HintAsync()
    {
        if (await dialogService.ShowSimpleDialogAsync(new()
        {
            Title = "Bist du dir sicher?",
            Content = "Verwende Tipps nur, wenn du wirklich verzweifelt bist und nicht weiter weißt.",
            CloseButtonText = "Abbrechen",
            PrimaryButtonText = "Fortfahren"
        }) != ContentDialogResult.Primary)
            return;

        await dialogService.ShowAlertAsync("Hier ist ein Tipp", CurrentQuestion?.Hint ?? "Etwas ist schief gelaufen. ('CurrentQuestion' is null)", "Okay");

        HintCount++;

        logger.LogInformation($"Tipp wurde angezeigt.");
    }


    void EvaluateResult()
    {

        logger.LogInformation($"Quiz Ergebnisse wurden ausgewertet.");
    }
}