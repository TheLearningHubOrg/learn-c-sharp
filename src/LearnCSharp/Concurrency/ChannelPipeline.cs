using System.Threading.Channels;
using LearnCSharp.Models;

namespace LearnCSharp.Concurrency;

public static class ChannelPipeline
{
    public static readonly IReadOnlyList<RoadmapStage> DefaultStages = new List<RoadmapStage>
    {
        new(1, "CLR Execution, JIT Compilation & IL Assembly", "Garbage collection generations, value types vs reference types", true),
        new(2, "Modern C# Syntax, Records & Pattern Matching", "Switch expressions, positional records, primary constructors", true),
        new(3, "Memory Safety, Span<T>, Memory<T> & ref struct", "Zero-allocation slicing, stackalloc, Unsafe APIs", true),
        new(4, "Asynchronous Programming & Task<T> Machinery", "AsyncStateMachine, SynchronizationContext, ValueTask<T>", false),
        new(5, "High-Throughput Concurrency & Channels", "System.Threading.Channels, Lock object, Interlocked primitives", false),
        new(6, "Reflection, Source Generators & Native AOT", "Compile-time code generation, trimming, self-contained binaries", false),
        new(7, "Staff .NET Architect Handbook & SRE", "Threadpool starvation, memory dump analysis with dotnet-dump, GC pause tuning", true)
    };

    public static async IAsyncEnumerable<RoadmapStage> StreamStagesAsync()
    {
        var channel = Channel.CreateBounded<RoadmapStage>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        _ = Task.Run(async () =>
        {
            foreach (var stage in DefaultStages)
            {
                await Task.Delay(20);
                await channel.Writer.WriteAsync(stage);
            }
            channel.Writer.Complete();
        });

        await foreach (var item in channel.Reader.ReadAllAsync())
        {
            yield return item;
        }
    }
}
