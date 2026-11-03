using System.Text.Json.Serialization;

namespace LearnCSharp.Models;

/// <summary>Where a roadmap stage sits on the path from beginner to staff engineer.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<StageLevel>))]
public enum StageLevel
{
    Foundations,
    Core,
    Advanced,
    Staff,
}

/// <summary>One stage of the guide. A positional record: immutable, value equality, deconstructable.</summary>
public sealed record RoadmapStage(int Number, string Title, string Focus, StageLevel Level);

/// <summary>Result of grouping the roadmap by level (Stage 2 exercise).</summary>
public sealed record LevelSummary(StageLevel Level, int StageCount, IReadOnlyList<int> StageNumbers);

public sealed record HealthReport(string Status, string Runtime);

/// <summary>What the process knows about the runtime it is executing on (Stage 1 exercise).</summary>
public sealed record RuntimeInfo(
    string Framework,
    string OperatingSystem,
    string Architecture,
    int ProcessorCount,
    bool ServerGarbageCollector);

/// <summary>Descriptive statistics for a list of numbers (Stage 3 exercise adds Median).</summary>
public sealed record StatsSummary(int Count, double Sum, double Mean, double Variance, double Median);

/// <summary>A parsed network endpoint. A record struct: value semantics without a heap allocation.</summary>
public readonly record struct NetworkEndpoint(string Scheme, string Host, int Port);
