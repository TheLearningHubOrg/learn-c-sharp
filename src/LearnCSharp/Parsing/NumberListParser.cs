using System.Globalization;
using LearnCSharp.Models;

namespace LearnCSharp.Parsing;

public static class NumberListParser
{
    public const int MaxItems = 10_000;

    /// <summary>
    /// Parses "1.5, 2, 3e2" into doubles. Splitting a span yields Range values that
    /// slice the original text, so no substring is allocated per item.
    /// </summary>
    public static Result<double[]> Parse(ReadOnlySpan<char> input)
    {
        if (input.IsWhiteSpace())
        {
            return Result.Fail<double[]>("Provide at least one number, for example ?values=1,2,3.");
        }

        var values = new List<double>();
        int position = 0;
        foreach (Range range in input.Split(','))
        {
            position++;
            ReadOnlySpan<char> token = input[range].Trim();

            // double.TryParse accepts "1e400" and returns Infinity, so check IsFinite too.
            if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                || !double.IsFinite(value))
            {
                return Result.Fail<double[]>($"Item {position} ('{token}') is not a finite number.");
            }

            if (values.Count == MaxItems)
            {
                return Result.Fail<double[]>($"At most {MaxItems} numbers are accepted.");
            }

            values.Add(value);
        }

        return Result.Ok(values.ToArray());
    }
}
