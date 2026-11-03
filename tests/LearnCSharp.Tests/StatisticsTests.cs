using LearnCSharp.Numerics;

namespace LearnCSharp.Tests;

public class StatisticsTests
{
    [Fact]
    public void Sum_works_for_integers_and_decimals()
    {
        Assert.Equal(10, Statistics.Sum<int>([1, 2, 3, 4]));
        Assert.Equal(0.6m, Statistics.Sum<decimal>([0.1m, 0.2m, 0.3m]));
    }

    [Fact]
    public void Mean_of_integers_truncates_because_integer_division_does()
    {
        Assert.Equal(2, Statistics.Mean<int>([1, 2, 4]));
        Assert.Equal(7m / 3m, Statistics.Mean<decimal>([1m, 2m, 4m]));
    }

    [Fact]
    public void Variance_is_population_variance()
    {
        Assert.Equal(4.0, Statistics.Variance<double>([2, 4, 4, 4, 5, 5, 7, 9]), precision: 10);
    }

    [Theory]
    [InlineData(new double[] { 3, 1, 2 }, 2)]
    [InlineData(new double[] { 4, 1, 3, 2 }, 2.5)]
    [InlineData(new double[] { 7 }, 7)]
    public void Median_handles_odd_and_even_lengths(double[] values, double expected)
    {
        double[] copy = [.. values];

        Assert.Equal(expected, Statistics.Median<double>(values));
        Assert.Equal(copy, values); // the input is not reordered
    }

    [Fact]
    public void Empty_input_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => Statistics.Mean<double>([]));
        Assert.Equal(0, Statistics.Sum<int>([]));
    }
}
