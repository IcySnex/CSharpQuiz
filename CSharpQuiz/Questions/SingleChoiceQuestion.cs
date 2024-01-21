using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class SingleChoiceQuestion : Question
{
    public SingleChoiceQuestion(
        string text,
        string hint,
        double points,
        string correctAnswer,
        params string[] choices) : base(
            text,
            "Single-Choice-Frage: Wähle eine der folgenden Antworten aus.",
            $"Single-Choice-Frage: +{points} Punkte für die richtige Antwort.",
            hint,
            points)
    {
        CorrectAnswer = correctAnswer;
        Choices = new ObservableCollection<string>(choices);
    }


    public string CorrectAnswer { get; }

    public ObservableCollection<string> Choices { get; }

    [ObservableProperty]
    string? selectedItem = null;


    public override double ReachedPoints
    {
        get
        {
            if (SelectedItem == CorrectAnswer)
                return Points;

            return 0;
        }
    }
}