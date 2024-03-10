using System.Windows.Controls;

namespace UI.SyntaxBox;

/// <summary>
/// A buffer of lines synchronized with TextBox.Text using the TextChanged event.
/// The buffer allows caching of syntax highlighting instructions for better
/// performance.
/// </summary>
public class LineBuffer : List<FormattedLine>
{
    /// <summary>
    /// Constructor defaulting initial capacity to 1024 lines.
    /// </summary>
    public LineBuffer(string InitialText)
        : this()
    {
        AddText(InitialText, 0, 0);
    }

    /// <summary>
    /// Constructor defaulting initial capacity to 1024 lines.
    /// </summary>
    public LineBuffer()
        : base(1024)
    {
        Add(new FormattedLine { Text = String.Empty });
    }


    /// <summary>
    /// Apples a TextChange as provided by TextChanged event from TextBox.
    /// </summary>
    /// <param name="Change"></param>
    /// <param name="AddedText"></param>
    public void ApplyChange(TextChange Change, string AddedText)
    {

        int startLine, startLineOffset;
        IdentifyChangeStart(Change, out startLine, out startLineOffset);

        if (Change.RemovedLength > 0)
        {
            RemoveText(Change, startLine, startLineOffset);
        }

        if (Change.AddedLength > 0)
        {
            AddText(AddedText, startLine, startLineOffset);
        }
    }

    public List<FormattedLine> GetLines(int FirstLine, int LastLine)
    {
        var lines = this
            .Skip(FirstLine)
            .Take(LastLine - FirstLine + 1)
            .ToList();

        return (lines);
    }


    private void IdentifyChangeStart(TextChange Change, out int StartLine, out int StartLineOffset)
    {
        int lineStart = 0, charCount = 0;
        for (StartLine = 0; StartLine < Count; StartLine++)
        {
            if (StartLine == Count - 1 || (charCount + this[StartLine].Text.Length) > Change.Offset)
                break;
            lineStart += this[StartLine].Text.Length;
            charCount += this[StartLine].Text.Length;
        }
        StartLineOffset = Change.Offset - lineStart;
    }

    private void RemoveText(TextChange Change, int StartLine, int StartLineOffset)
    {
        // Delete old lines
        int removedChars = 0, currentLine = StartLine;
        int prevLine = 0;
        while (removedChars < Change.RemovedLength)
        {
            int toRemove;
            if (removedChars == 0) //currentLine == startLine) // FirstLine
            {
                toRemove = Math.Min(Change.RemovedLength - removedChars, this[currentLine].Text.Length - StartLineOffset);
                string prefix = this[currentLine].Text.Substring(0, StartLineOffset);
                string postfix = this[currentLine].Text.Substring(StartLineOffset + toRemove);
                this[currentLine].Text = prefix + postfix;

            }
            else
            {
                toRemove = Math.Min(Change.RemovedLength - removedChars, this[currentLine].Text.Length - prevLine);
                this[currentLine].Text =
                      this[currentLine].Text.Substring(0, prevLine)
                    + this[currentLine].Text.Substring(toRemove + prevLine);
            }
            this[currentLine].LineFormatInstructions = null;
            this[currentLine].BlockFormatInstructions = null;
            removedChars += toRemove;

            // Merge with next line
            if (currentLine < Count - 1
                && !this[currentLine].Text.EndsWith(Environment.NewLine.Last().ToString()))
            {
                prevLine = this[currentLine].Text.Length;
                this[currentLine + 1].Text = this[currentLine].Text + this[currentLine + 1].Text;
                this[currentLine].Text = String.Empty;
            }
            else
            {
                prevLine = 0;
            }

            if (this[currentLine].Text.Length == 0 && currentLine < Count - 1)
            {
                RemoveAt(currentLine);
            }
            else
            {
                currentLine++;
            }
        }
    }

    private void AddText(string AddedText, int StartLine, int StartLineOffset)
    {
        // Identify new lines
        var addedLines = AddedText.GetLines(0, Int32.MaxValue, out int count)
            .Select((x) => new FormattedLine { Text = x.Text })
            .ToList();

        // Insert new lines
        string prefix = this[StartLine].Text.Substring(0, StartLineOffset);
        string postfix = this[StartLine].Text.Substring(StartLineOffset);
        var firstLine = addedLines[0];
        var lastLine = addedLines[addedLines.Count - 1];
        firstLine.Text = prefix + firstLine.Text;
        lastLine.Text = lastLine.Text + postfix;
        RemoveAt(StartLine);
        InsertRange(StartLine, addedLines);
    }


    /// <summary>
    /// Overrides ToString returning the all lines as a joined text.
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        string.Join("", this.Select((x) => x.Text).ToArray());
}