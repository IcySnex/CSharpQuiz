using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class MultipleChoiceQuestion : Question
{
    public MultipleChoiceQuestion(
        string text,
        string hint,
        double points,
        string[] correctAnswers,
        params string[] choices) : base(
            text,
            "Multiple-Choice-Frage: Wähle mehrere der folgenden Antworten aus.",
            $"Multiple-Choice-Frage: +{points / correctAnswers.Length} Punkte für richtige Antworten, -{points / correctAnswers.Length / 2} Punkte für falsche Antworten.",
            hint,
            points)
    {
        CorrectAnswers = correctAnswers;
        Choices = new ObservableCollection<string>(choices);
    }


    public string[] CorrectAnswers { get; }

    public ObservableCollection<string> Choices { get; }

    public List<string> SelectedItems { get; } = new();


    public override double ReachedPoints
    {
        get
        {
            double points = 0;
            double pointsPerQuestion = Points / CorrectAnswers.Length;

            foreach (string selectedChoice in SelectedItems)
                points = CorrectAnswers.Contains(selectedChoice) ? points + pointsPerQuestion : points - (pointsPerQuestion / 2);

            return Math.Max(0, points);
        }
    }


    [RelayCommand]
    void OnListViewSelectionChanged(
        ListView list)
    {
        SelectedItems.Clear();

        foreach (string item in list.SelectedItems)
            SelectedItems.Add(item);
    }

    [RelayCommand]
    void OnListViewLoaded(
        ListView list)
    {
        list.SelectedItems.Clear();

        foreach (string item in SelectedItems)
            list.SelectedItems.Add(item);
    }
}