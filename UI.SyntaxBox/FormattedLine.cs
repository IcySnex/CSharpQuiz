namespace UI.SyntaxBox;

public class FormattedLine
{
    public string Text { get; set; }

    public List<FormatInstruction> LineFormatInstructions { get; set; }

    public List<FormatInstruction> BlockFormatInstructions { get; set; }
}