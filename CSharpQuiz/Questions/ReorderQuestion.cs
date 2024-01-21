using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class ReorderQuestion : Question
{
    public ReorderQuestion(
        string text,
        string hint,
        double points,
        params string[] items) : base(
            text,
            "Reihenfolge-Frage: Bringe die folgenden Antworten in die richtige Reihenfolge.",
            $"Reihenfolge-Frage: +{points} Punkte für die richtige Reihenfolge.",
            hint,
            points)
    {
        Items = new ObservableCollection<string>(items);
    }


    public ObservableCollection<string> Items { get; }


    public override double ReachedPoints
    {
        get
        {
            return 0;
        }
    }
}