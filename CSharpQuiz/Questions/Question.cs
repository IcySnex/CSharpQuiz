namespace CSharpQuiz.Questions;

public abstract class Question
{
    protected Question(
        string text,
        string typeNote,
        string typeAnswerNote,
        string hint,
        double points)
    {
        Text = text;
        TypeNote = typeNote;
        TypeAnswerNote = typeAnswerNote;
        Hint = hint;
        Points = points;
    }


    public string Text { get; }

    public string TypeNote { get; }

    public string TypeAnswerNote { get; }
    
    public string Hint { get; }

    public double Points { get; }


    public abstract double ReachedPoints { get; }
}