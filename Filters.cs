
public class LengthFilter : FilterBase
{
    public override string Name => "Length";

    protected override (int, bool, string) Evaluate(string password)
    {
        int points = (int)(Math.Min(password.Length, 12) * 2.5);
        bool passed = password.Length >= 8;
        return (points, passed, passed
            ? $"{password.Length} characters"
            : $"Only {password.Length} characters. Use at least 8.");
    }
}

public class LowercaseFilter : FilterBase
{
    public override string Name => "Lowercase letters";

    protected override (int, bool, string) Evaluate(string password)
    {
        bool has = password.Any(char.IsLower);
        return (has ? 10 : 0, has, has ? "Has lowercase letters" : "Add a lowercase letter");
    }
}

public class UppercaseFilter : FilterBase
{
    public override string Name => "Uppercase letters";

    protected override (int, bool, string) Evaluate(string password)
    {
        bool has = password.Any(char.IsUpper);
        return (has ? 15 : 0, has, has ? "Has uppercase letters" : "Add an uppercase letter");
    }
}

public class DigitFilter : FilterBase
{
    public override string Name => "Digits";

    protected override (int, bool, string) Evaluate(string password)
    {
        bool has = password.Any(char.IsDigit);
        return (has ? 15 : 0, has, has ? "Has digits" : "Add a number");
    }
}

public class SymbolFilter : FilterBase
{
    public override string Name => "Symbols";

    protected override (int, bool, string) Evaluate(string password)
    {
        bool has = password.Any(c => !char.IsLetterOrDigit(c));
        return (has ? 20 : 0, has, has ? "Has symbols" : "Add a symbol such as ! or #");
    }
}

public class CommonPasswordFilter : FilterBase
{
    public override string Name => "Common password check";

    private static readonly HashSet<string> Common = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "password1", "123456", "123456789", "12345678", "qwerty",
        "abc123", "letmein", "admin", "welcome", "iloveyou", "111111"
    };

    protected override (int, bool, string) Evaluate(string password)
    {
        if (password.Length > 0 && Common.Contains(password))
            return (-50, false, "This is one of the most common passwords");

        return (10, true, "Not on the common passwords list");
    }
}
