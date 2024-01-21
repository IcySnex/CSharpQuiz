using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Questions;
using CSharpQuiz.Views.Questions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace CSharpQuiz.ViewModels;

public partial class QuizViewModel : ObservableObject
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

        Questions = GenerateQuestions();
        CurrentView = new WelcomeView(this);

        logger.LogInformation("QuizViewModel wurde initialisiert.");
    }


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsResultEvaluated))]

    UserControl currentView;

    public bool IsResultEvaluated =>
        CurrentView.GetType() == typeof(ResultView);


    List<Question> GenerateQuestions()
    {
        return new()
        { 
            new SingleChoiceQuestion(
                text: "Test QuesSSSSSSSStion",
                hint: "Nothing at all lol",
                points: 3,
                correctAnswer: "Second",
                "First", "Second", "Third"),
            new MultipleChoiceQuestion(
                text: "Select MULTIPLe WOPWOWOW",
                hint: "nfffffffffffff",
                points: 5,
                correctAnswers: new[] { "223534fgd", "sss" },
                "sddsdsds", "223534fgd", "fddfdf", "sss"),
            new ReorderQuestion(
                text: "Select MULTIPLe WOPWOWOW",
                hint: "nfffffffffffff",
                points: 5,
                "sddsdsds", "223534fgd", "fddfdf", "sss")
        };
    }


    public List<Question> Questions { get; private set; }


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentQuestion))]
    [NotifyPropertyChangedFor(nameof(IsLastQuestion))]
    [NotifyCanExecuteChangedFor(nameof(GoNextCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoBackCommand))]
    int? currentQuestionIndex = null;

    public Question? CurrentQuestion =>
        Questions is null || CurrentQuestionIndex is null ? null : Questions[CurrentQuestionIndex.Value];

    public bool IsLastQuestion =>
        CurrentQuestionIndex is not null && CurrentQuestionIndex == Questions.Count - 1;

    public bool CanGoNext =>
        CurrentQuestionIndex is not null && CurrentQuestionIndex < Questions.Count - 1;

    public bool CanGoBack =>
        CurrentQuestionIndex is not null && CurrentQuestionIndex > 0;


    [ObservableProperty]
    bool isQuizRunning = false;

    [ObservableProperty]
    int hintCount = 0;

    [ObservableProperty]
    string timeEvolved = "00:00";


    readonly Stopwatch stopwatch = new();
    readonly DispatcherTimer timeEvolvedTimer = new() { Interval = TimeSpan.FromSeconds(1) };


    [ObservableProperty]
    double points = 0;

    [ObservableProperty]
    double reachedPoints = 0;

    [ObservableProperty]
    int correctAnswersCount = 0;


    [RelayCommand(CanExecute = nameof(CanGoNext))]
    void GoNext()
    {
        if (CurrentQuestionIndex is null)
            return;

        CurrentQuestionIndex++;

        logger.LogInformation($"Es wurde zur nächsten Frage gewechselt.");
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    void GoBack()
    {
        if (CurrentQuestionIndex is null)
            return;

        CurrentQuestionIndex--;

        logger.LogInformation($"Es wurde zur letzten Frage gewechselt.");
    }


    [RelayCommand]
    void Start()
    {
        IsQuizRunning = true;
        stopwatch.Start();
        timeEvolvedTimer.Start();

        CurrentQuestionIndex = 0;

        CurrentView = new QuestionsView(this);

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
        stopwatch.Stop();
        timeEvolvedTimer.Stop();
        CurrentQuestionIndex = null;

        logger.LogInformation($"Quiz wurde beendet.");

        EvaluateResult();
    }

    [RelayCommand]
    void Reset()
    {
        Points = 0;
        ReachedPoints = 0;
        CorrectAnswersCount = 0;
        HintCount = 0;
        TimeEvolved = "00:00";
        stopwatch.Reset();

        Questions = GenerateQuestions();
        CurrentView = new WelcomeView(this);

        logger.LogInformation($"Quiz wurde zurück gesetzt.");
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
        foreach (Question question in Questions)
        {
            Points += question.Points;

            double newPoints = question.ReachedPoints;
            ReachedPoints += newPoints;

            if (newPoints == question.Points)
                CorrectAnswersCount++;
        }

        CurrentView = new ResultView(this);

        logger.LogInformation($"Quiz Ergebnisse wurden ausgewertet.");
    }
}