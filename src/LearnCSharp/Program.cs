using System.Runtime.InteropServices;
using LearnCSharp.Concurrency;
using LearnCSharp.Models;

Console.WriteLine(new string('=', 70));
Console.WriteLine("  Learn C# 12 / .NET 8 Systems Architecture Masterclass");
Console.WriteLine(new string('=', 70));

Console.WriteLine($"\n[CLR Runtime] .NET Version: {Environment.Version}");
Console.WriteLine($"[Environment] OS: {RuntimeInformation.OSDescription} ({RuntimeInformation.ProcessArchitecture})");
Console.WriteLine($"[Garbage Collector] Server GC: {System.Runtime.GCSettings.IsServerGC}, LatencyMode: {System.Runtime.GCSettings.LatencyMode}");

Console.WriteLine("\n[Channel Pipeline] Streaming 7-Stage Pedagogical Roadmap:");
int completedCount = 0;

await foreach (var stage in ChannelPipeline.StreamStagesAsync())
{
    var symbol = stage.Completed ? "✓" : "○";
    if (stage.Completed) completedCount++;
    Console.WriteLine($"  [{symbol}] Stage {stage.StageNumber}: {stage.Title}");
    Console.WriteLine($"      Domain: {stage.DomainFocus}");
}

int total = ChannelPipeline.DefaultStages.Count;
int percentage = (int)((double)completedCount / total * 100);
Console.WriteLine($"\n[Curriculum Progress] Mastered: {completedCount} / {total} stages ({percentage}%)");

Result<string> operationResult = new Result<string>.Success("All channels completed without deadlocks.");
var message = operationResult switch
{
    Result<string>.Success s => $"Execution Succeeded: {s.Data}",
    Result<string>.Failure f => $"Execution Failed: {f.Error}",
    _ => "Unknown result state"
};

Console.WriteLine($"\n[Pattern Matching Result] {message}");
