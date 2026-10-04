var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();  
app.UseStaticFiles();

var pipeline = new Pipeline()
    .Add(new LengthFilter())
    .Add(new LowercaseFilter())
    .Add(new UppercaseFilter())
    .Add(new DigitFilter())
    .Add(new SymbolFilter())
    .Add(new CommonPasswordFilter());

app.MapPost("/api/check", (CheckRequest request) =>
{
    var result = pipeline.Run(request.Password ?? "");

    var strength = result.Score switch
    {
        < 40 => "Weak",
        < 70 => "Medium",
        _ => "Strong"
    };

    return Results.Ok(new { result.Score, Strength = strength, result.Steps });
});

app.Run();

record CheckRequest(string? Password);
