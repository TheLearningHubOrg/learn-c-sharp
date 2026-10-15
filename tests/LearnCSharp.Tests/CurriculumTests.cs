using Xunit;
using LearnCSharp.Concurrency;
using LearnCSharp.Models;

namespace LearnCSharp.Tests;

public class CurriculumTests
{
    [Fact]
    public async Task ChannelPipeline_StreamsAllSevenStages()
    {
        var stages = new List<RoadmapStage>();
        await foreach (var stage in ChannelPipeline.StreamStagesAsync())
        {
            stages.Add(stage);
        }

        Assert.Equal(7, stages.Count);
        Assert.Equal(1, stages.First().StageNumber);
        Assert.Equal(7, stages.Last().StageNumber);
    }

    [Fact]
    public void ResultType_PatternMatchingEvaluatesCorrectly()
    {
        Result<int> result = new Result<int>.Success(100);

        var value = result switch
        {
            Result<int>.Success s => s.Data,
            Result<int>.Failure => -1,
            _ => 0
        };

        Assert.Equal(100, value);
    }

    [Fact]
    public void RoadmapStages_HasCompletedFoundations()
    {
        var completed = ChannelPipeline.DefaultStages.Count(s => s.Completed);
        Assert.True(completed >= 2);
    }
}
