using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpQuiz.Helpers;

public class UnknownTypes
{
    public static bool AreEqual(
        object a,
        object b)
    {
        if (a.GetType() != b.GetType())
            return false;

        return a switch
        {
            int intA when b is int intB =>
                intA == intB,
            string stringA when b is string stringB =>
                stringA == stringB,
            IEnumerable enumerableA when b is IEnumerable enumerableB =>
                Enumerable.SequenceEqual(enumerableA.Cast<object>(), enumerableB.Cast<object>()),
            _ => ReferenceEquals(a, b),
        };
    }


    public static string ToString(object? obj)
    {
        if (obj is IEnumerable<object> enumerable)
            return EnumerableToString(enumerable, 1);

        return obj?.ToString() ?? "null";
    }

    static string EnumerableToString<T>(IEnumerable<T> enumerable, int indentationLevel)
    {
        var sb = new StringBuilder();
        sb.Append("[\n");

        bool isFirst = true;
        foreach (var element in enumerable)
        {
            if (!isFirst)
                sb.Append(",\n");

            sb.Append(Indent(indentationLevel));
            if (element is IEnumerable<object> enumerableElement)
                sb.Append(EnumerableToString(enumerableElement, indentationLevel + 1));
            else
                sb.Append(element?.ToString() ?? "null");

            isFirst = false;
        }

        sb.Append($"\n{Indent(indentationLevel - 1)}]");
        return sb.ToString();
    }

    static string Indent(int level) =>
        new(' ', level * 4);
}