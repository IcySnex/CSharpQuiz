namespace CSharpQuiz.Questions;

public abstract class Question(
    string text,
    string typeNote,
    string typeAnswerNote,
    string hint,
    double points)
{
    public string Text { get; } = text;

    public string TypeNote { get; } = typeNote;

    public string TypeAnswerNote { get; } = typeAnswerNote;

    public string Hint { get; } = hint;

    public double Points { get; } = points;


    public abstract double ReachedPoints { get; }
}