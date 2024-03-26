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
using CSharpQuiz.Views.Dialogs;
using CSharpQuiz.Services;

namespace CSharpQuiz.ViewModels;

public partial class QuizViewModel : ObservableObject
{
    readonly ILogger<QuizViewModel> logger;
    readonly ContentDialogService dialogService;
    readonly Navigation navigation;

    public QuizViewModel(
        ILogger<QuizViewModel> logger,
        ContentDialogService dialogService,
        Navigation navigation)
    {
        this.logger = logger;
        this.dialogService = dialogService;
        this.navigation = navigation;

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
            new SingleChoiceQuestion(
                text: "Von welchem Unternehmen wurde C# entwickelt?",
                hint: "Das selbe Unternehmen hat auch die Xbox entwickelt.",
                points: 2,
                correctAnswer: "Microsoft",
                    "Google",
                    "Oracle Corporation",
                    "Microsoft",
                    "IBM",
                    "Apple",
                    "SAP"),
            new TrueOrFalseQuestion(
                text: "Allgemeine Fakten über C#",
                hint: "C# ist eine moderne, objektorientierte Sprache aus dem Jahr 2000 mit vielen Ähnlichkeiten zu Java.",
                points: 5,
                    new("Die Main-Mehtode in C# ist erforderlich und dient als Einstiegspunkt für die Ausführund des Programms", true),
                    new("C# wurde im Jahr 2005 mit dem .NET Framework veröffentlicht", false),
                    new("LINQ steht für \"Language-Integrated Network Query\" und wird hauptsächlich für Netzwerkoperationen verwendet", false),
                    new("C# wird stets weiterentwickelt mit regulären Updates & neuen Funktionen", true),
                    new("Vieles von C# lässt sich mit Java vergleichen", true)),
            new SingleChoiceQuestion(
                text: "Wer ist der Hauptarchitekt der Sprache C#?",
                hint: "Er ist ein dänischer Entwickler, welcher auch TypeScript entworfen hat.",
                points: 1,
                correctAnswer: "Anders Hejlsberg",
                    "Bjarne Stroustrup",
                    "Anders Hejlsberg",
                    "Dennis Ritchie",
                    "James Gosling"),
            new MultipleChoiceQuestion(
                text: "Was ist die .NET-Plattform?",
                hint: "Die .NET-Plattform erlaubt es C#-Anwendungen überhaupt erst zu laufen, ohne diese würde nichts funktionnieren.",
                points: 3,
                correctAnswers: [
                    "Die .NET-Plattform bietet Werkzeuge für die Bereitstellung & Ausführung von Anwendungen",
                    "Der Vorteil der .NET-Plattform ist, dass es Plattformunabhängigkeit ermöglicht",
                    "Die Laufzeitumgebung von .NET Anwendungen nennt man \"Common Language Runtime\" (CLR)"
                    ],
                    "Die .NET-Plattform bietet Werkzeuge für die Bereitstellung & Ausführung von Anwendungen",
                    "Nur C# wird von der .NET-Plattform unterstützt",
                    "Der Vorteil der .NET-Plattform ist, dass es Plattformunabhängigkeit ermöglicht",
                    "Die Laufzeitumgebung von .NET Anwendungen nennt man \"Common Language Runtime\" (CLR)",
                    "Die \"Common Language Runtime\" (CLR) lässt sich nicht mit der \"Java Virtual Machine\" (JVM) vergleichen"),
            new CodingQuestion(
                text: "Addiere die Beträge der gegebene Zahlen",
                hint: "Dieses Problem ist relativ simpel:\nÜberprüfe zuerst, ob die beiden Zahlen negativ sind und falls so, multipliziere sie mit -1, damit sie nicht mehr negativ sind (Pro-Tipp: Du kannst sogar eine kleine Helfer Methode schreiben, damit du dich nicht wiederholen musst).",
                points: 8,
                defaultMethod: "AddiereBeträge",
                argSets: CreateAddiereBeträgeArgSets(5, -50, 50),
                expectedDelegate: (int a, int b) => Math.Abs(a) + Math.Abs(b),
                """
                public class MatheHelfer
                {
                    // In dieser Methode sollst du die Beträge der gegebenen Zahlen 'a' & 'b' addieren
                    // Heißt also, die Zahlen dürfen nicht negativ sein
                    public static int AddiereBeträge(int a, int b)
                    {
                    }
                }
                """,
                """
                public class MatheHelfer
                {
                    public static int AddiereBeträge(int a, int b)
                    {
                        int betragVonA = Betrag(a);
                        int betragVonB = Betrag(b);
                        return betragVonA + betragVonB;
                    }
            
                    static int Betrag(int zahl) =>
                        zahl < 0 ? -zahl : zahl;
                }
                """),
            new SingleChoiceQuestion(
                text: "Was ist der Hauptunterschied zwischen einer Klasse und einem Objekt?",
                hint: "Also das haben wir eigentlich im Unterricht gemacht, das solltest du wissen...",
                points: 2,
                correctAnswer: "Eine Klasse ist eine Vorlage zur Erzeugung von Objekten.",
                    "Eine Klasse ist eine Instanz eines Objekts.",
                    "Eine Klasse ist eine Vorlage zur Erzeugung von Objekten.",
                    "Eine Klasse kann keine Methoden haben, während ein Objekt dies kann.",
                    "Eine Klasse kann keine Eigenschaften haben, während ein Objekt dies kann."),
            new TrueOrFalseQuestion(
                text: "C# in der Praxis",
                hint: "Mit C# lässt sich so gut wie alles & überall machen.",
                points: 6,
                    new("Mit C# lassen sich nur GUI-Anwendungen schreiben", false),
                    new("C# läuft nur auf Windows-Geräten", false),
                    new("C# ermöglicht Cross-Plattform-Apps", true),
                    new("Künstliche Intelligenz kann man in C#-Anwendungen noch nicht integrieren", false),
                    new("Mit C# kann man sogar Web-Anwendungen entwickeln", true),
                    new("Durch Unity oder MonoGame können gesamte 2D/3D Spiele entwickelt werden", true)),
            new ReorderQuestion(
                text: "Wie wird C# mit der .NET-Plattform ausgeführt?",
                hint: "Zuerst muss ein Quellcode überhaupt existieren, dieser wird dann zu einem IL-Code kompiliert und von der CLR in machinenabhängigen Code übersetzt, wobei die CLR auch die Ausführung überwacht.",
                points: 5,
                correctItemsOrder: [
                    "Der Entwickler schreibt einen C#-Quellcode",
                    "Der C#-Quellcode wird zu einem Zwischencode (IL) kompiliert",
                    "Die CLR nimmt den IL-Code & übersetzt ihn während der Laufzeit in maschinenabhängigen Code (JIT)",
                    "Die CLR überwacht die Ausführung und bietet Funktionen wie \"Garbage Collection\" oder Fehlerbehandlung",
                    "Während der Ausführung interagiert die C#-Anwendungen mit der .NET Klassenbibilothek"
                    ],
                    "Der C#-Quellcode wird zu einem Zwischencode (IL) kompiliert",
                    "Die CLR überwacht die Ausführung und bietet Funktionen wie \"Garbage Collection\" oder Fehlerbehandlung",
                    "Der Entwickler schreibt einen C#-Quellcode",
                    "Während der Ausführung interagiert die C#-Anwendungen mit der .NET Klassenbibilothek",
                    "Die CLR nimmt den IL-Code & übersetzt ihn während der Laufzeit in maschinenabhängigen Code (JIT)"),
            new MultipleChoiceQuestion(
                text: "Was ist asynchrone Programmierung in C# & wofür wird es verwendet?",
                hint: "Asynchrone Programmierung ist ziemlich nützlich um aufwendige Aufgaben nebenbei woanders laufen zu lassen, wo sie niemanden stören, wie wenn du deinen kleinen Bruder mit deinem Handy wegschickst, damit er dich nicht beim Spielen stört.",
                points: 3,
                correctAnswers: [
                    "Wichtiger Bestandteil moderner Andwendungen",
                    "Ermöglicht langlaufende/aufwendige Operationen auf einem anderen Thread zu berechnen",
                    "Erlaubt paralele Ausführung von Aufgaben"
                    ],
                    "Beeinträchtigt Leistung & Resourcennutzung negativ",
                    "Ermöglicht langlaufende/aufwendige Operationen auf einem anderen CPU-Core zu berechnen",
                    "Wichtiger Bestandteil moderner Andwendungen",
                    "Wird selten bei GUI-Anwendungen verwendet",
                    "Ermöglicht langlaufende/aufwendige Operationen auf einem anderen Thread zu berechnen",
                    "Erlaubt paralele Ausführung von Aufgaben"),
            new TrueOrFalseQuestion(
                text: "Unterschiede & Gemeinsamkeiten zu Java",
                hint: "Das sollte relativ einfach sein, falls du keine Ahnung hast, musst du wohl raten :)",
                points: 5,
                    new("C# & Java verwenden beide die JVM (\"Java Virtual Machine\") als Laufzeitumgebung", false),
                    new("In C# ist der Typ \"string\" ein allgemeiner Datentyp", true),
                    new("C# erlaubt native Nullable-Typen, während Java einen \"Wrapper\" benötigt", true),
                    new("Eigenschaften (Attribute) sind in Java deutlich simpler & einfacher", false),
                    new("Der Syntax bei C# lässt sich oft vereinfachen/kürzen z.B. bei Methoden oder Schleifen", true)),
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

        await EvaluateResultAsync();
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

        navigation.Navigate("Startseite");
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


    async Task EvaluateResultAsync()
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
        await Task.Delay(300);

        ResultOverviewDialog dialog = new(ReachedPoints, Points, CorrectAnswersCount, Questions.Count, HintCount, TimeEvolved);
        await dialogService.ShowAsync(dialog, default);

        logger.LogInformation($"Quiz Ergebnisse wurden ausgewertet.");
    }
}