using LearnCSharp.Models;

namespace LearnCSharp.Curriculum;

public static class RoadmapQueries
{
    /// <summary>Looks up one stage. Absence is an expected outcome, so it is a Failure, not an exception.</summary>
    public static Result<RoadmapStage> Find(IEnumerable<RoadmapStage> stages, int number) =>
        stages.FirstOrDefault(stage => stage.Number == number) is { } found
            ? Result.Ok(found)
            : Result.Fail<RoadmapStage>($"Stage {number} does not exist. Valid stages are 1-7.");

    /// <summary>Groups stages by level, in enum order (Stage 2 exercise solution).</summary>
    public static IReadOnlyList<LevelSummary> SummarizeByLevel(IEnumerable<RoadmapStage> stages) =>
        stages
            .GroupBy(stage => stage.Level)
            .OrderBy(group => group.Key)
            .Select(group => new LevelSummary(
                group.Key,
                group.Count(),
                group.Select(stage => stage.Number).Order().ToArray()))
            .ToList();
}
