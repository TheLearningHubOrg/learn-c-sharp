using LearnCSharp.Models;

namespace LearnCSharp.Curriculum;

public static class Roadmap
{
    /// <summary>
    /// The seven stages of the guide. A collection expression that targets
    /// IReadOnlyList lets the compiler pick a read-only backing type.
    /// </summary>
    public static IReadOnlyList<RoadmapStage> Stages { get; } =
    [
        new(1, "Toolchain, Runtime & C# Fundamentals", "dotnet CLI, project files, built-in types, control flow", StageLevel.Foundations),
        new(2, "Methods, Types, Collections & LINQ", "classes, structs, records, pattern matching, generic collections", StageLevel.Foundations),
        new(3, "OOP, Generics, Memory & Error Handling", "interfaces, constraints, generic math, Span, exceptions, IDisposable", StageLevel.Core),
        new(4, "Async, Concurrency & Parallelism", "Task, ValueTask, IAsyncEnumerable, Channels, the ThreadPool", StageLevel.Core),
        new(5, "I/O, Serialization, HTTP, Data & Testing", "streams, System.Text.Json, HttpClient, EF Core, xUnit", StageLevel.Advanced),
        new(6, "Performance, Diagnostics, Security & Deployment", "GC tuning, BenchmarkDotNet, dotnet-counters, Native AOT, containers", StageLevel.Advanced),
        new(7, "Staff C# Interview Handbook", "internals, failure modes, design trade-offs", StageLevel.Staff),
    ];
}
