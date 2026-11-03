using System.Text.Json;
using System.Text.Json.Serialization;
using LearnCSharp.Models;

namespace LearnCSharp.Serialization;

/// <summary>
/// The System.Text.Json source generator writes the serialization code for these
/// types at compile time, so the API does not need reflection to produce JSON.
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(HealthReport))]
[JsonSerializable(typeof(RuntimeInfo))]
[JsonSerializable(typeof(RoadmapStage))]
[JsonSerializable(typeof(IAsyncEnumerable<RoadmapStage>))]
[JsonSerializable(typeof(IReadOnlyList<LevelSummary>))]
[JsonSerializable(typeof(StatsSummary))]
[JsonSerializable(typeof(NetworkEndpoint))]
internal sealed partial class AppJsonContext : JsonSerializerContext;
