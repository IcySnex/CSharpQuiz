using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class TrueOrFalseQuestion(
    string text,
    string hint,
    double points,
    params TrueOrFalseItem[] items) : Question(
        text,
        "Wahr/Falsch-Frage: Kreuze wahr oder falsch an.",
        $"Wahr/Falsch-Frage: +{points / items.Length:#.##} Punkte für richtige Antworten.",
        hint,
        points)
{
    public TrueOrFalseItem[] Items { get; } = items;


    public override double ReachedPoints
    {
        get
        {
            double points = 0;
            double pointsPerQuestion = Points / Items.Length;

            foreach (TrueOrFalseItem item in Items)
                if (item.IsCorrect ? item.IsTrue : item.IsFalse)
                    points += pointsPerQuestion;

            double roundedPoints = Math.Floor(points / 0.25) * 0.25;
            return roundedPoints;
        }
    }
}


public partial class TrueOrFalseItem(
    string text,
    bool isCorrect) : ObservableObject
{
    public string Text { get; } = text;

    public bool IsCorrect { get; set; } = isCorrect;


    [ObservableProperty]
    bool isTrue;

    partial void OnIsTrueChanged(
        bool value)
    {
        if (value)
            IsFalse = false;
    }

    [ObservableProperty]
    bool isFalse;

    partial void OnIsFalseChanged(
        bool value)
    {
        if (value)
            IsTrue = false;
    }
}
