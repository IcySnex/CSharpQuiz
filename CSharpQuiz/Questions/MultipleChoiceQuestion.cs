using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class MultipleChoiceQuestion(
    string text,
    string hint,
    double points,
    string[] correctAnswers,
    params string[] choices) : Question(
        text,
        "Multiple-Choice-Frage: Wähle mehrere der folgenden Antworten aus.",
        $"Multiple-Choice-Frage: +{points / correctAnswers.Length:0.##} Punkte für richtige Antworten, -{points / correctAnswers.Length / 2} Punkte für falsche Antworten.",
        hint,
        points)
{
    public string[] CorrectAnswers { get; } = correctAnswers;

    public ObservableCollection<string> Choices { get; } = new(choices);

    public List<string> SelectedItems { get; } = [];


    public override double ReachedPoints
    {
        get
        {
            double points = 0;
            double pointsPerQuestion = Points / CorrectAnswers.Length;

            foreach (string selectedChoice in SelectedItems)
                points = CorrectAnswers.Contains(selectedChoice) ? points + pointsPerQuestion : points - (pointsPerQuestion / 2);

            double roundedPoints = Math.Floor(Math.Max(0, points) / 0.25) * 0.25;
            return roundedPoints;
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