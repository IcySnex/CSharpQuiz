using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class ReorderQuestion(
    string text,
    string hint,
    double points,
    string[] correctItemsOrder,
    params string[] items) : Question(
        text,
        "Reihenfolge-Frage: Ordne die folgenden Antworten in die richtige Reihenfolge.",
        $"Reihenfolge-Frage: +{points} Punkte für die richtige Reihenfolge, abhängig von der Ähnlichkeit.",
        hint,
        points)
{
    public string[] CorrectItemsOrder { get; } = correctItemsOrder;

    public ObservableCollection<string> Items { get; } = new(items);


    public override double ReachedPoints
    {
        get
        {
            int kendallTauDistance = 0;
            for (int i = 0; i < CorrectItemsOrder.Length - 1; i++)
                for (int j = i + 1; j < CorrectItemsOrder.Length; j++)
                    if (Items.IndexOf(CorrectItemsOrder[i]) > Items.IndexOf(CorrectItemsOrder[j]))
                        kendallTauDistance++;

            int maxKendallTauDistance = CorrectItemsOrder.Length * (CorrectItemsOrder.Length - 1) / 2;

            double similarity = 1.0 - (double)kendallTauDistance / maxKendallTauDistance;
            double points = similarity * Points;

            double roundedPoints = Math.Floor(points / 0.25) * 0.25;
            return roundedPoints;
        }
    }
}