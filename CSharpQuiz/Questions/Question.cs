namespace CSharpQuiz.Questions;

public abstract class Question
{
    protected Question(
        string text,
        string typeNote,
        string hint,
        int points)
    {
        Text = text;
        TypeNote = typeNote;
        Hint = hint;
        Points = points;
    }


    public string Text { get; }

    public string TypeNote { get; }
    
    public string Hint { get; }

    public int Points { get; }


    public abstract int EvaluatePoints();
}