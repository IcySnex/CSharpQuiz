using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class TrueOrFalseQuestion : Question
{
    public TrueOrFalseQuestion(
        string text,
        string hint,
        double points,
        params TrueOrFalseItem[] items) : base(
            text,
            "Wahr/Falsch-Frage: Kreuze wahr oder falsch an.",
            $"Wahr/Falsch-Frage: +{points / items.Length:#.##} Punkte für richtige Antworten.",
            hint,
            points)
    {
        Items = items;
    }


    public TrueOrFalseItem[] Items { get; }


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


public partial class TrueOrFalseItem : ObservableObject
{
    public TrueOrFalseItem(
        string text,
        bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }

    public string Text { get; }

    public bool IsCorrect { get; set; }


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
