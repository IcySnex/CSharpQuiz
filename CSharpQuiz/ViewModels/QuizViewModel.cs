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
using Wpf.Ui.Extensions;

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
        Random rnd = new();

        return
        [
            new SingleChoiceQuestion(
                text: "Test Frage",
                hint: "Das ist ein sehr hilfreicher Tipp",
                points: 3,
                correctAnswer: "Zweitens",
                "Erstens", "Zweitens", "Drittens"),
            new MultipleChoiceQuestion(
                text: "Jetzt kannst du sogar mehr auswählen",
                hint: "es gibt keinen",
                points: 2,
                correctAnswers: ["a", "d"],
                "a", "b", "c", "d", "e"),
            new ReorderQuestion(
                text: "RICHTIGE REIHENFOLGE",
                hint: "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
                points: 5,
                correctItemsOrder: ["1", "2", "3 ", "4", "5"],
                "4", "1", "3 ", "5", "2"),
            new TrueOrFalseQuestion(
                text: "ordne es zu?!?!?!",
                hint: "was denkst du denn?",
                points: 4,
                new("A ist ein Buchstabe", true), new("5 + 3 = 7", false), new("C# ist toll!", true), new("Hier musst du wohl raten, tja", false)),
            new CodingQuestion(
                text: "Addiere die Beträge der gegebene Zahlen",
                hint: "Überprüfe zuerst, ob die beiden Zahlen negativ sind und falls so, mache sie positiv bevor du sie addierst.",
                points: 10,
                defaultMethod: "AddiereBetraege",
                argSets:
                [
                    [rnd.Next(-50, 50), rnd.Next(-50, 50)],
                    [rnd.Next(-50, 50), rnd.Next(-50, 50)],
                    [rnd.Next(-50, 50), rnd.Next(-50, 50)]
                ],
                expectedDelegate: (int a, int b) => Math.Abs(a) + Math.Abs(b),
                """
                public class Beispiel
                {
                    public static int AddiereBetraege(int a, int b)
                    {
                        // Addiere die Beträge der beiden Zahlen 'a' & 'b'.
                        //
                        // Bsp:
                        // a = 5, b = 10 => Ergebnis: 15
                        // a = 2, b = -4 => Ergebnis: 6
                        // a = -6, b = -2 => Ergebnis: 8
                    }
                }
                """,
                """
                public class Beispiel
                {
                    public static int AddiereBetraege(int a, int b) =>
                        Betrag(a) + Betrag(b);

                    static int Betrag(int zahl) =>
                        zahl < 0 ? -zahl : zahl;
                }
                """)
        ];
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