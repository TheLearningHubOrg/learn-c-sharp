using System.Numerics;

namespace LearnCSharp.Numerics;

/// <summary>
/// Generic math: one implementation that works for int, long, double, decimal and
/// any other type implementing INumber. Static abstract interface members supply
/// T.Zero, the operators and T.CreateChecked at compile time, with no boxing.
/// </summary>
public static class Statistics
{
    public static T Sum<T>(ReadOnlySpan<T> values) where T : INumber<T>
    {
        T total = T.Zero;
        foreach (T value in values)
        {
            total += value;
        }

        return total;
    }

    public static T Mean<T>(ReadOnlySpan<T> values) where T : INumber<T>
    {
        ThrowIfEmpty(values);
        return Sum(values) / T.CreateChecked(values.Length);
    }

    /// <summary>Population variance: the mean of squared distances from the mean.</summary>
    public static T Variance<T>(ReadOnlySpan<T> values) where T : INumber<T>
    {
        T mean = Mean(values);
        T squares = T.Zero;
        foreach (T value in values)
        {
            T distance = value - mean;
            squares += distance * distance;
        }

        return squares / T.CreateChecked(values.Length);
    }

    /// <summary>Middle value of the sorted input (Stage 3 exercise solution). The input is not modified.</summary>
    public static T Median<T>(ReadOnlySpan<T> values) where T : INumber<T>
    {
        ThrowIfEmpty(values);
        T[] sorted = values.ToArray();
        sorted.AsSpan().Sort();

        int middle = sorted.Length / 2;
        return sorted.Length % 2 == 1
            ? sorted[middle]
            : (sorted[middle - 1] + sorted[middle]) / T.CreateChecked(2);
    }

    private static void ThrowIfEmpty<T>(ReadOnlySpan<T> values)
    {
        if (values.IsEmpty)
        {
            throw new ArgumentException("At least one value is required.", nameof(values));
        }
    }
}
