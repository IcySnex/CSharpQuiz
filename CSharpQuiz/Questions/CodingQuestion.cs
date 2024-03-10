using CommunityToolkit.Mvvm.ComponentModel;

namespace CSharpQuiz.Questions;

[ObservableObject]
public partial class CodingQuestion : Question
{
    public CodingQuestion(
        string text,
        string hint,
        double points) : base(
            text,
            "Programmier-Frage: Vervollständige den unteren Programmcode.",
            $"Programmier-Frage: +{points} Punkte, wenn die Aufgabe erfüllt ist.",
            hint,
            points)
    {
    }


    public override double ReachedPoints
    {
        get
        {
            return 0;
        }
    }
}