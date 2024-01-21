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
        int points,
        string[] correctAnswers,
        params string[] choices) : base(text, "Multiple-Choice-Frage: Wähle mehrere der folgenden Antworten aus.", hint, points)
    {
        CorrectAnswers = correctAnswers;
        Choices = new ObservableCollection<string>(choices);
    }


    public string[] CorrectAnswers { get; }

    public ObservableCollection<string> Choices { get; }

    public List<string> SelectedItems { get; } = new();


    public override int EvaluatePoints()
    {
        int points = 0;
        foreach (string selectedChoice in SelectedItems)
            points = CorrectAnswers.Contains(selectedChoice) ? points + 1 : points - 1;

        return Math.Max(0, points);
    }


    [RelayCommand]
    void OnListViewSelectionChanged(SelectionChangedEventArgs e)
    {
        foreach (string item in e.RemovedItems)
            SelectedItems.Remove(item);

        foreach (string item in e.AddedItems)
            SelectedItems.Add(item);
    }
}