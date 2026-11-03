using LearnCSharp.Concurrency;
using LearnCSharp.Curriculum;
using LearnCSharp.Models;

namespace LearnCSharp.Tests;

public class CurriculumTests
{
    [Fact]
    public async Task Channel_pipeline_streams_every_item_in_order()
    {
        var received = new List<int>();
        await foreach (RoadmapStage stage in ChannelPipeline.StreamAsync(Roadmap.Stages, capacity: 2))
        {
            received.Add(stage.Number);
        }

        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, received);
    }

    [Fact]
    public async Task Leaving_the_stream_early_does_not_hang_the_producer()
    {
        int seen = 0;
        await foreach (int value in ChannelPipeline.StreamAsync(Enumerable.Range(0, 1_000), capacity: 1))
        {
            seen++;
            if (value == 3)
            {
                break;
            }
        }

        Assert.Equal(4, seen);
    }

    [Fact]
    public async Task Producer_exceptions_reach_the_consumer()
    {
        static IEnumerable<int> Broken()
        {
            yield return 1;
            throw new InvalidOperationException("source failed");
        }

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await foreach (int _ in ChannelPipeline.StreamAsync(Broken()))
            {
            }
        });
        Assert.Equal("source failed", error.Message);
    }

    [Fact]
    public void Find_returns_failure_for_unknown_stage()
    {
        Assert.IsType<Result<RoadmapStage>.Success>(RoadmapQueries.Find(Roadmap.Stages, 3));
        Assert.IsType<Result<RoadmapStage>.Failure>(RoadmapQueries.Find(Roadmap.Stages, 99));
    }

    [Fact]
    public void Summary_groups_stages_by_level_in_enum_order()
    {
        IReadOnlyList<LevelSummary> summary = RoadmapQueries.SummarizeByLevel(Roadmap.Stages);

        Assert.Equal(new[] { StageLevel.Foundations, StageLevel.Core, StageLevel.Advanced, StageLevel.Staff }, summary.Select(s => s.Level));
        Assert.Equal(7, summary.Sum(s => s.StageCount));
        Assert.Equal(new[] { 1, 2 }, summary[0].StageNumbers);
    }

    [Fact]
    public void Result_match_and_map_follow_the_case()
    {
        Result<int> ok = Result.Ok(20);
        Result<int> failed = Result.Fail<int>("nope");

        Assert.Equal("40", ok.Map(x => x * 2).Match(x => x.ToString(), e => e));
        Assert.Equal("nope", failed.Map(x => x * 2).Match(x => x.ToString(), e => e));
        Assert.Equal(Result.Ok(20), ok); // records compare by value
    }
}
