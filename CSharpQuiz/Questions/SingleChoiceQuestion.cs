using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class SingleChoiceQuestion(
    string text,
    string hint,
    double points,
    string correctAnswer,
    params string[] choices) : Question(
        text,
        "Single-Choice-Frage: Wähle eine der folgenden Antworten aus.",
        $"Single-Choice-Frage: +{points} Punkte für die richtige Antwort.",
        hint,
        points)
{
    public string CorrectAnswer { get; } = correctAnswer;

    public ObservableCollection<string> Choices { get; } = new(choices);

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