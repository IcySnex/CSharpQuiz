using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Questions;
using CSharpQuiz.Views.Questions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using System.Linq;
using CSharpQuiz.Shared;

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
        this.dialogService = dialogService; ;

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


    static List<Question> GenerateQuestions()
    {
        Random rnd = new();

        object?[][] CreateAddiereBeträgeArgSets(int count, int min, int max)
        {
            object?[][] result = new object?[count][];
            for (int i = 0; i < count; i++)
                result[i] = [rnd.Next(min, max), rnd.Next(min, max)];

            return result;
        }

        object?[][] CreateFiltereBücherArgSets()
        {
            Buch[] books = [
                new("Stolz und Vorurteil", "Jane Austen", 10.99, true),
                new("Verstand und Gefühl", "Jane Austen", 9.99, false),
                new("Emma", "Jane Austen", 12.49, true),
                new("Große Erwartungen", "Charles Dickens", 8.79, true),
                new("Eine Geschichte aus zwei Städten", "Charles Dickens", 11.29, false),
                new("Oliver Twist", "Charles Dickens", 7.99, true),
                new("Shining", "Stephen King", 13.99, true),
                new("Es", "Stephen King", 14.49, true),
                new("The Stand – Das letzte Gefecht", "Stephen King", 12.99, false),
                new("Harry Potter und der Stein der Weisen", "J.K. Rowling", 15.99, true),
                new("Harry Potter und die Kammer des Schreckens", "J.K. Rowling", 16.49, true),
                new("Harry Potter und der Gefangene von Askaban", "J.K. Rowling", 17.99, false),
                new("Mord im Orientexpress", "Agatha Christie", 11.99, true),
                new("Und dann gabs keines mehr", "Agatha Christie", 10.49, true),
                new("Die Morde des Herrn ABC", "Agatha Christie", 13.79, false),
                new("1984", "George Orwell", 9.49, true),
                new("Farm der Tiere", "George Orwell", 8.99, false),
                ];
            rnd.Shuffle(books);

            object?[][] result = [
                [books, "Jane Austen"],
                [books, "Charles Dickens"],
                [books, "Stephen King"],
                [books, "J.K. Rowling"],
                [books, "Agatha Christie"],
                [books, "George Orwell"],
                ];

            return result;
        }


        return
        [
            new CodingQuestion(
                text: "Filtere die Bücher nach Autor/Verfügbarkeit & sortiere nach Preis",
                hint: "Die Aufgabe scheint ziemlich groß & schwer zu sein, aber mithilfe von LINQ ist sie relativ einfach:\nNutze den Syntax 'from buch in bücher' um mit LINQ zu starten.\nDann kannst du mit 'where' überprüfen ob der Autor der selbe ist, wie der gegebene Parameter. Zudem kannst du hier gleich nach der Verfügbarkeit filtern in dem du den Boolean-Operator '&&' verwendest.\nAnschließend nur noch mit 'orderby' nach dem Preis sortieren und das wars schon :)",
                points: 8,
                defaultMethod: "FiltereBücher",
                argSets: CreateFiltereBücherArgSets(),
                expectedDelegate: (Buch[] bücher, string autor) => bücher.Where(buch => buch.Autor == autor && buch.IstVerfügbar).OrderBy(buch => buch.Preis),
                """
                using CSharpQuiz.Shared;
                using System.Collections.Generic;
                using System.Linq;
                
                // Das ist eine Helfer Klasse für eine Bücherei
                public class Bücherei
                {
                    // In dieser Methode sollst du alle Bücher von dem gegeben Autor heraussuchen
                    // Außerdem sollst du nur die Bücher zurückgeben, welche noch verfügbar sind
                    // Zuletzt sollst du nach dem Preis sortieren (= vom günstigsten zum teuersten)
                    public static IEnumerable<Buch> FiltereBücher(Buch[] bücher, string autor)
                    {
                    }
                }
                                
                // So sieht die Klasse 'Buch' aus:
                // public class Buch
                // {
                //     public string Titel { get; }
                //     public string Autor { get; }
                //     public double Preis { get; set; }
                //     public bool IstVerfügbar { get; set; }
                // }
                """,
                """
                using CSharpQuiz.Shared;
                using System.Collections.Generic;
                using System.Linq;

                public class Bücherei
                {
                    public static IEnumerable<Buch> FiltereBücher(Buch[] bücher, string autor)
                    {
                        var gefiltert = from buch in bücher
                                        where buch.Autor == autor && buch.IstVerfügbar
                                        orderby buch.Preis
                                        select buch;
                        return gefiltert;
                    }
                }
                """),
        ];
    }


    public List<Question> Questions { get; private set; }


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentQuestion))]
    [NotifyCanExecuteChangedFor(nameof(GoNextCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoBackCommand))]
    int? currentQuestionIndex = null;

    public Question? CurrentQuestion =>
        Questions is null || CurrentQuestionIndex is null ? null : Questions[CurrentQuestionIndex.Value];

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
        if (await dialogService.ShowSimpleDialogAsync(new()
        {
            Title = "Bist du dir sicher?",
            Content = "Wenn du das Quiz beendest werden unbeantwortete Fragen als falsch makiert & du gelangst direkt zur Auswertung.",
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

        await Task.Delay(300);
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