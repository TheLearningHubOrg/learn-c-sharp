using LearnCSharp.Models;
using LearnCSharp.Parsing;

namespace LearnCSharp.Tests;

public class NumberListParserTests
{
    [Fact]
    public void Parses_numbers_with_spaces_and_exponents()
    {
        var result = NumberListParser.Parse(" 1.5, 2 ,3e2");

        var success = Assert.IsType<Result<double[]>.Success>(result);
        Assert.Equal(new double[] { 1.5, 2, 300 }, success.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Blank_input_fails(string? input)
    {
        Assert.IsType<Result<double[]>.Failure>(NumberListParser.Parse(input));
    }

    [Theory]
    [InlineData("1,two,3", "Item 2 ('two')")]
    [InlineData("1,,3", "Item 2 ('')")]
    [InlineData("1e400", "Item 1 ('1e400')")]
    public void Bad_items_are_reported_by_position(string input, string expectedFragment)
    {
        var failure = Assert.IsType<Result<double[]>.Failure>(NumberListParser.Parse(input));
        Assert.Contains(expectedFragment, failure.Error);
    }
}

public class EndpointParserTests
{
    [Theory]
    [InlineData("https://api.example.com", "https", "api.example.com", 443)]
    [InlineData("HTTP://localhost:5000/health", "http", "localhost", 5000)]
    [InlineData("redis://cache.internal", "redis", "cache.internal", 6379)]
    public void Parses_valid_endpoints(string input, string scheme, string host, int port)
    {
        var success = Assert.IsType<Result<NetworkEndpoint>.Success>(EndpointParser.Parse(input));
        Assert.Equal(new NetworkEndpoint(scheme, host, port), success.Value);
    }

    [Theory]
    [InlineData("example.com")]
    [InlineData("https://:443")]
    [InlineData("https://example.com:0")]
    [InlineData("https://example.com:70000")]
    [InlineData("gopher://example.com")]
    public void Rejects_invalid_endpoints(string input)
    {
        Assert.IsType<Result<NetworkEndpoint>.Failure>(EndpointParser.Parse(input));
    }
}
