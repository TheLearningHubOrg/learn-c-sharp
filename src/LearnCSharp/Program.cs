using System.Runtime;
using System.Runtime.InteropServices;
using LearnCSharp.Concurrency;
using LearnCSharp.Curriculum;
using LearnCSharp.Models;
using LearnCSharp.Numerics;
using LearnCSharp.Parsing;
using LearnCSharp.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// One port, read from PORT, bound on every interface so a container can reach it.
string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Use the compile-time JSON serializers first; reflection stays as a fallback.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default));
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapGet("/health", () => new HealthReport("ok", RuntimeInformation.FrameworkDescription));

// Stage 1 exercise: report what the runtime knows about itself.
app.MapGet("/api/runtime", () => new RuntimeInfo(
    RuntimeInformation.FrameworkDescription,
    RuntimeInformation.OSDescription,
    RuntimeInformation.ProcessArchitecture.ToString(),
    Environment.ProcessorCount,
    GCSettings.IsServerGC));

// Streams the stages through a bounded channel; the response is written as items arrive.
app.MapGet("/api/roadmap", (CancellationToken ct) =>
    ChannelPipeline.StreamAsync(Roadmap.Stages, cancellationToken: ct));

// Stage 2 exercise: LINQ grouping.
app.MapGet("/api/roadmap/summary", () => RoadmapQueries.SummarizeByLevel(Roadmap.Stages));

app.MapGet("/api/roadmap/{number:int}", Results<Ok<RoadmapStage>, ProblemHttpResult> (int number) =>
    RoadmapQueries.Find(Roadmap.Stages, number) switch
    {
        Result<RoadmapStage>.Success found => TypedResults.Ok(found.Value),
        Result<RoadmapStage>.Failure missing => TypedResults.Problem(missing.Error, statusCode: StatusCodes.Status404NotFound),
        _ => throw new InvalidOperationException("Unknown result."),
    });

app.MapGet("/api/stats", Results<Ok<StatsSummary>, ProblemHttpResult> (string? values) =>
    NumberListParser.Parse(values).Map(Summarize).Match<Results<Ok<StatsSummary>, ProblemHttpResult>>(
        summary => TypedResults.Ok(summary),
        error => TypedResults.Problem(error, statusCode: StatusCodes.Status400BadRequest)));

app.MapGet("/api/endpoint", Results<Ok<NetworkEndpoint>, ProblemHttpResult> (string? uri) =>
    EndpointParser.Parse(uri) switch
    {
        Result<NetworkEndpoint>.Success parsed => TypedResults.Ok(parsed.Value),
        Result<NetworkEndpoint>.Failure failed => TypedResults.Problem(failed.Error, statusCode: StatusCodes.Status400BadRequest),
        _ => throw new InvalidOperationException("Unknown result."),
    });

app.Run();

static StatsSummary Summarize(double[] numbers)
{
    ReadOnlySpan<double> span = numbers;
    return new StatsSummary(
        span.Length,
        Statistics.Sum(span),
        Statistics.Mean(span),
        Statistics.Variance(span),
        Statistics.Median(span));
}

// Makes the implicit Program class visible to WebApplicationFactory in the tests.
public partial class Program;
