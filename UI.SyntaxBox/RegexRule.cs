using System.Text.RegularExpressions;
using System.Windows.Media;

namespace UI.SyntaxBox;

public class RegexRule : ISyntaxRule
{
    private Regex _regex = null;


    public int RuleId { get; set; }

    /// <summary>
    /// The driver operation to apply (Line | Block | FullText).
    /// </summary>
    public DriverOperation Op { get; set; } = DriverOperation.None;

    /// <summary>
    /// Matches the rule against the provided text.
    /// Used internally, shouldn't be called by user code.
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public IEnumerable<FormatInstruction> Match(string Text)
    {
        var regex = GetRegex();
        var matches = regex.Matches(Text);

        foreach (Match match in matches)
        {
            yield return new FormatInstruction
            {
                RuleId = RuleId,
                FromChar = match.Index,
                Length = match.Length,
                Foreground = Foreground,
                Background = Background,
                Outline = Outline
            };
        }
    }


    /// <summary>
    /// Background brush
    /// </summary>
    public Brush Background { get; set; }

    /// <summary>
    /// Foreground brush.
    /// </summary>
    public Brush Foreground { get; set; }

    /// <summary>
    /// Outline pen
    /// </summary>
    public Pen Outline { get; set; }

    /// <summary>
    /// The regex pattern to match.
    /// </summary>
    public string Pattern { get; set; }


    private Regex GetRegex()
    {
        _regex ??= new Regex(Pattern);

        return _regex;
    }
}