namespace LearnCSharp.Models;

public record RoadmapStage(
    int StageNumber,
    string Title,
    string DomainFocus,
    bool Completed = false
);

public abstract record Result<T>
{
    public record Success(T Data) : Result<T>;
    public record Failure(string Error) : Result<T>;
}
