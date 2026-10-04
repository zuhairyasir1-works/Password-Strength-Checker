public record StepResult(string Filter, string Message, int Points, int RunningScore, bool Passed);


public record PasswordData(string Password, int Score, List<StepResult> Steps);


public interface IFilter
{
    string Name { get; }
    PasswordData Process(PasswordData input);
}

public abstract class FilterBase : IFilter
{
    public abstract string Name { get; }

    protected abstract (int points, bool passed, string message) Evaluate(string password);

    public PasswordData Process(PasswordData input)
    {
        var (points, passed, message) = Evaluate(input.Password);
        var score = Math.Max(0, input.Score + points);

        var steps = new List<StepResult>(input.Steps)
        {
            new(Name, message, points, score, passed)
        };

        return input with { Score = score, Steps = steps };
    }
}


public class Pipeline
{
    private readonly List<IFilter> _filters = new();

    public Pipeline Add(IFilter filter)
    {
        _filters.Add(filter);
        return this;
    }

    public PasswordData Run(string password) =>
        _filters.Aggregate(
            new PasswordData(password, 0, new List<StepResult>()),
            (data, filter) => filter.Process(data));
}
