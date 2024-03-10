namespace UI.SyntaxBox;

public class TextLine
{
    public int LineNumber;

    public int StartIndex;

    public string Text;

    public int EndIndex => StartIndex + Text?.Length ?? 0;
}