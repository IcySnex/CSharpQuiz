using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class ReorderQuestion : Question
{
    public ReorderQuestion(
        string text,
        string hint,
        double points,
        string[] correctItemsOrder,
        params string[] items) : base(
            text,
            "Reihenfolge-Frage: Ordne die folgenden Antworten in die richtige Reihenfolge.",
            $"Reihenfolge-Frage: +{points} Punkte für die richtige Reihenfolge, abhängig von der Ähnlichkeit.",
            hint,
            points)
    {
        CorrectItemsOrder = correctItemsOrder;
        Items = new(items);
    }


    public string[] CorrectItemsOrder { get; }

    public ObservableCollection<string> Items { get; }


    public override double ReachedPoints
    {
        get
        {
            int penalty = 0;
            for (int i = 0; i < CorrectItemsOrder.Length; i++)
                if (i >= Items.Count || Items[i] != CorrectItemsOrder[i])
                    penalty += CorrectItemsOrder.Length;

            double maxPenalty = CorrectItemsOrder.Length * CorrectItemsOrder.Length;
            double points = Math.Max(0, (maxPenalty - penalty * 0.5) / maxPenalty) * Points;
            double roundedPoints = Math.Floor(points / 0.25) * 0.25;

            return roundedPoints;
        }
    }
}