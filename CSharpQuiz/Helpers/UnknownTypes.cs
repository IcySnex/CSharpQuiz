using System;
using System.Collections;
using System.Linq;

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

    public static string ToString(
        object? obj)
    {
        if (obj is null)
            return "null";

        Type type = obj.GetType();
        if (type.IsPrimitive || obj is string || obj is decimal)
            return obj.ToString()!;

        if (obj is Array array)
            return EnumerableToString(array);

        return type.Name;
    }

    static string EnumerableToString(
        Array array)
    {
        string result = "[";
        for (int i = 0; i < array.Length; i++)
        {
            result += ToString(array.GetValue(i));
            if (i < array.Length - 1)
                result += ", ";
        }
        result += "]";

        return result;
    }
}