# Password Strength Checker

A small web app, built in C# with ASP.NET Core, that scores a password out of 100 and shows exactly how the score was reached. It is built to demonstrate the **Pipe and Filter** architectural style.

## How it works

A password is sent from the browser to the server and travels through a chain of independent filters. Each filter does one job (for example, "does it contain a digit?"), adds its points to the running score, and hands the data on to the next filter.

```
Password
   |
   v
[ Length ] -> [ Lowercase ] -> [ Uppercase ] -> [ Digits ] -> [ Symbols ] -> [ Common check ]
                                                                                    |
                                                                                    v
                                                                   Score + Strength + step-by-step results
```

The web page draws this as a vertical pipe with one station per filter, so you can watch each filter's contribution.

### Why this is Pipe and Filter

| Concept | In this project |
|---|---|
| **Data** | `PasswordData` (the password, the running score, and a list of step results) |
| **Filter** | Any class implementing `IFilter` (`LengthFilter`, `DigitFilter`, ...) |
| **Pipe** | The `Pipeline` class, which passes the output of one filter into the next |
| **Independence** | Filters know nothing about each other and can be added, removed, or reordered |

## Project structure

```
PasswordChecker/
  PasswordChecker.csproj   Project file (set TargetFramework to your .NET version)
  Program.cs               Builds the pipeline and exposes POST /api/check
  Pipeline.cs              PasswordData, IFilter, FilterBase, and the Pipeline class
  Filters.cs               The six filters
  wwwroot/
    index.html             Frontend (HTML, CSS, and JavaScript in one file)
```

## Requirements

- .NET SDK (check with `dotnet --version`)
- Any modern web browser

## Running it

1. Open a terminal in the project folder (the one containing `PasswordChecker.csproj`).
2. If needed, edit `<TargetFramework>` in the `.csproj` to match your SDK (for example `net9.0`).
3. Run:

   ```
   dotnet run
   ```

4. Open the URL printed in the terminal (for example `http://localhost:5123`).
5. Press `Ctrl+C` in the terminal to stop the app.

## Scoring

The maximum score is 100.

| Filter | Points | Rule |
|---|---|---|
| Length | up to +30 | 2.5 points per character, counted up to 12 characters |
| Lowercase letters | +10 | At least one lowercase letter |
| Uppercase letters | +15 | At least one uppercase letter |
| Digits | +15 | At least one digit |
| Symbols | +20 | At least one character that is not a letter or digit |
| Common password check | +10 or -50 | +10 if not on the common list, -50 if it is |

The score never drops below 0. The verdict is **Weak** below 40, **Medium** from 40 to 69, and **Strong** from 70 up.

## API

`POST /api/check`

Request:

```json
{ "password": "hello" }
```

Response (steps trimmed for brevity):

```json
{
  "score": 32,
  "strength": "Weak",
  "steps": [
    { "filter": "Length", "message": "5 characters", "points": 12, "runningScore": 12, "passed": false },
    { "filter": "Lowercase letters", "message": "Has lowercase letters", "points": 10, "runningScore": 22, "passed": true }
  ]
}
```

## Adding a new filter

1. Add a class to `Filters.cs`:

   ```csharp
   public class RepeatedCharactersFilter : FilterBase
   {
       public override string Name => "Repeated characters";

       protected override (int, bool, string) Evaluate(string password)
       {
           bool hasRepeat = password.Zip(password.Skip(1), (a, b) => a == b).Any(x => x);

           return hasRepeat
               ? (-10, false, "Avoid repeating the same character in a row")
               : (0, true, "No repeated characters in a row");
       }
   }
   ```

2. Plug it into the pipe in `Program.cs`:

   ```csharp
   .Add(new RepeatedCharactersFilter())
   ```

3. Optional: update the sentence in `wwwroot/index.html` that says "six filters".

No other file needs to change, which is the main benefit of this architecture.

## Notes

- This is a teaching project. Avoid typing a real password you use elsewhere, even though it is only sent to your own local server.
- The "common passwords" list is a short sample, not a complete dictionary.
