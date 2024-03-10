using System.Windows.Media;

namespace UI.SyntaxBox;

/// <summary>
/// This rule is used to match a list of keywords wit input text.
/// It is ~10x faster than using regex for the same purpose.
/// </summary>
public class KeywordRule : ISyntaxRule
{
    private AhoCorasickSearch engine = null;


    public int RuleId { get; set; }

    public DriverOperation Op { get; set; } = DriverOperation.Line;

    /// <summary>
    /// Matches the rule against the provided text.
    /// Used internally, shouldn't be called by user code.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public IEnumerable<FormatInstruction> Match(string text)
    {
        var engine = GetEngine();
        var matched = engine.FindAll(text).ToList();
        var instructions = matched
            .Select((x) => new FormatInstruction
            {
                FromChar = x.Position,
                Length = x.Length,

                RuleId = RuleId,
                Foreground = Foreground,
                Background = Background,
                Outline = Outline
            }).ToList();
        return instructions;

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

    public string Keywords { get; set; }

    public bool WholeWordsOnly { get; set; } = true;


    private AhoCorasickSearch GetEngine()
    {
        if (engine == null)
        {
            var keywordList = (Keywords ?? string.Empty)
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select((x) => x.Trim())
                .ToList();
            engine = new(keywordList, WholeWordsOnly);
        }

        return engine;
    }
}