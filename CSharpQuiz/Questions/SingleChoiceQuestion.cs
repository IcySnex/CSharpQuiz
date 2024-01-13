using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class SingleChoiceQuestion : Question
{
    public SingleChoiceQuestion(
        string text,
        string hint,
        int points,
        int correctIndex,
        params string[] choices) : base(text, "Single-Choice-Frage: Wähle einer der folgenden Antworten aus.", hint, points)
    {
        CorrectIndex = correctIndex;
        Choices = new ObservableCollection<string>(choices);
    }


    public int CorrectIndex { get; }

    public ObservableCollection<string> Choices { get; }

    [ObservableProperty]
    int selectedIndex = 0;


    public override int EvaluatePoints()
    {
        if (SelectedIndex == CorrectIndex)
            return Points;

        return 0;
    }
}