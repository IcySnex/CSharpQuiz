using System.Data;

namespace UI.SyntaxBox;

/// <summary>
/// Canonical algorithm for quickly finding all occurances of muiltiple 
/// keywords in a longer text.
/// </summary>
public class AhoCorasickSearch
{
    private const int MAXCHARS = 256;

    private readonly int maxStates;

    private int[] fail;
    private HashSet<int>[] output;
    private List<int[]> gotoList;

    private List<string> dictionary;

    private bool matchWholeWords = true;
    private bool overlappingMatches = false;


    /// <summary>
    /// Creates an Aho-Corasick keyword search automation.
    /// </summary>
    /// <param name="Dictionary">The list of words t osearch for.</param>
    /// <param name="MatchWholeWords">
    /// Ensures that matches start and end on a word boundary (alphanumeric or '').
    /// Default = <c>true</c>.
    /// </param>
    /// <param name="OverlappingMatches">
    /// If enabled, overlapping matches are allowed, e.g. elsif => elsif, if.
    /// Default = <c>false</c>.
    /// </param>
    public AhoCorasickSearch(
        List<string> Dictionary,
        bool MatchWholeWords = true,
        bool OverlappingMatches = false)
    {
        matchWholeWords = MatchWholeWords;
        overlappingMatches = OverlappingMatches;

        dictionary = Dictionary
            .OrderByDescending((x) => x.Length)
            .ToList();

        maxStates = dictionary.Sum((word) => word.Length) + 1;

        InitiateTables();
        InitiateAutomation();
    }


    /// <summary>
    /// Creates and initiates all automation tables
    /// </summary>
    private void InitiateTables()
    {
        // Create and initiate the OUTPUT table to 0
        output = new HashSet<int>[maxStates];
        for (int i = 0; i < maxStates; i++)
            output[i] = new HashSet<int>();

        // Create and initiate the FAIL table to 1
        fail = new int[maxStates];
        for (int i = 0; i < maxStates; i++)
            fail[i] = -1;

        // Create and initiate the GOTO table to -1.
        int[] template = new int[MAXCHARS];
        for (int i = 0; i < MAXCHARS; i++)
            template[i] = -1;

        gotoList = new List<int[]>(maxStates);
        gotoList.Add(template);
        for (int i = 1; i < maxStates; i++)
            gotoList.Add((int[])template.Clone());
    }

    /// <summary>
    /// Initiates the automation, loading it into the created tables.
    /// </summary>
    private void InitiateAutomation()
    {
        int states = 1;
        for (int i = 0; i < dictionary.Count; i++)
        {
            string word = dictionary[i];

            int currentState = 0;
            foreach (char ch in word)
            {
                if (ch > MAXCHARS)
                    throw new InvalidOperationException($"Only the first {MAXCHARS} characters are allowed!");

                if (gotoList[currentState][ch] == -1)
                    gotoList[currentState][ch] = states++;

                currentState = gotoList[currentState][ch];
            }

            output[currentState].Add(i);
        }

        Queue<int> queue = new Queue<int>();


        for (int i = 0; i < MAXCHARS; i++)
        {
            if (gotoList[0][i] == -1)
                gotoList[0][i] = 0;
            else
            {
                int gotoValue = gotoList[0][i];
                fail[gotoValue] = 0;
                queue.Enqueue(gotoValue);
            }
        }

        while (queue.Count > 0)
        {
            int state = queue.Dequeue();

            for (int i = 0; i < MAXCHARS; i++)
            {
                if (gotoList[state][i] == -1)
                    continue;

                int failure = fail[state];

                while (gotoList[failure][i] == -1)
                    failure = fail[failure];

                failure = gotoList[failure][i];
                fail[gotoList[state][i]] = failure;

                output[gotoList[state][i]].AddRange(output[failure]);

                queue.Enqueue(gotoList[state][i]);
            }
        }
    }


    /// <summary>
    /// Finds the next state based on the current state and the next input 
    /// character.
    /// </summary>
    /// <param name="currentState"></param>
    /// <param name="inputChar"></param>
    /// <returns></returns>
    private int NextState(int currentState, int inputChar)
    {
        int current = currentState;
        while (gotoList[current][inputChar] == -1)
        {
            current = fail[current];
        }

        return gotoList[current][inputChar];
    }


    /// <summary>
    /// Scans text returning all occurances of any keyword in the dictionary.
    /// </summary>
    /// <param name="text"></param>
    public IEnumerable<Substring> FindAll(string text)
    {
        Substring prevMatch = null;

        int current = 0;
        for (int i = 0; i < text.Length; i++)
        {
            current = NextState(current, text[i]);

            if (output[current].Count == 0)
                continue;

            for (int j = 0; j < dictionary.Count; j++)
            {
                if (!output[current].Contains(j))
                    continue;

                int length = dictionary[j].Length;
                int firstChar = i - length + 1;

                if (!(!matchWholeWords || text.IsStartWordBoundary(firstChar) && text.IsEndWordBoundary(i)))
                    continue;

                Substring substring = new()
                {
                    Position = firstChar,
                    Length = length,
                    Value = dictionary[j]
                };

                if (overlappingMatches)
                    yield return substring;
                else
                {
                    if (prevMatch != null && prevMatch.Position != substring.Position)
                        yield return prevMatch;

                    prevMatch = substring;
                    break;
                }
            }
        }

        if (prevMatch != null)
            yield return prevMatch;
    }
}