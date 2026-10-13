# Statistics Calculator

A C# WPF coursework application with four functional screens: descriptive statistics and charts, correlation and least-squares regression, binomial probability, and a CSV data workspace.

## Requirements

- Windows and the .NET 10 SDK, approved by the tutor.
- A Visual Studio installation supporting the .NET 10 target, or the .NET CLI.
- NuGet access for the first restore.
- DB Browser for SQLite for the optional database login demonstration.

Existing packages are Entity Framework Core SQLite, Microsoft.Data.Sqlite.Core, ScottPlot.WPF, and NUnit. Eugene's features do not add another package.

## Build, run, and test

Open WpfApp.slnx in a compatible Visual Studio installation, or run from the project folder in PowerShell:

```powershell
dotnet --version
dotnet build .\WpfApp.slnx -m:1
dotnet run --project .\WpfApp\WpfApp.csproj
```

Close the application, then run:

```powershell
dotnet test .\NUnitTest\NUnitTest.csproj
```

The existing SkiaSharp NU1701 warning does not itself stop compilation. Verify the actual Windows charts before presentation.

## Startup and optional login demonstration

The current application starts at the main menu. Logout opens the login screen. Startup creates the SQLite schema using Entity Framework; it does not seed an account. Users.db is relative to the application's working directory.

1. Run and close the normal application once.
2. Locate the generated database:

```powershell
Get-ChildItem -Path . -Filter Users.db -Recurse | Select-Object FullName
```

3. Open that database in DB Browser for SQLite. In Execute SQL, run samples/demo_user.sql, then select Write Changes.
4. On a fresh database, run the application in the same way, click Logout, and sign in as demo with password Demo123!. Also try an incorrect password. The script leaves an existing demo account unchanged.

This is a coursework demonstration account. Describe the existing startup and login behavior accurately in the report.

## Four-screen demonstration

| Screen | Example | Expected behavior |
| --- | --- | --- |
| Descriptive Statistics | 1,2,3,4,5 | Mean 3, median 3, population SD about 1.414, sample SD about 1.581, and charts |
| Correlation and Regression | (1,3), (2,5), (3,7) | Slope 2, intercept 1, correlation 1 |
| Binomial Probability | n=4, k=1, p=0.5 | Exactly 25%, at most 31.25%, at least 93.75% |
| CSV Workspace | samples/example_numbers.csv | Five numbers; positive-only displays 2.5, 7, 10 |

Close the probability or CSV dialog to return to the menu. The other screens retain their Return buttons. Resize all four screens and check that controls remain accessible.

## CSV workflow

Import the sample with First row is a header checked, or load the sample text already in the window. Choose a filter and order, then export or copy the displayed values. Export writes the displayed data after filtering and sorting. Paste copied numbers into Descriptive Statistics.

Use one numeric column, one number per row, and a decimal point. Uncheck the header option when the first row is a number. Imports allow up to 10,000 numbers; imported files are limited to 2 MB. Invalid imports keep the previous valid dataset.

## Code requirement examples

| Requirement | Implementation |
| --- | --- |
| Method overloading | Median for list/array; ParseCsv and ToCsv with header choices |
| Two interfaces | ICsvReader and ICsvWriter, implemented by CsvDataService and used by CsvWorkspace |
| LINQ lambda | PositiveCsvValues uses Where(value => value > 0) |
| Generic collections | List<double> and ObservableCollection<DataPoint> |
| NUnit | StatisticsCalculatorTests and EugeneFeatureTests |
| External tools | WPF, EF Core/SQLite, and ScottPlot |

StatisticsCalculator contains mathematical calculations. CsvDataService contains CSV parsing, exporting, and filtering. The probability window delegates calculations to the calculator; the CSV window uses reader/writer contracts for import and export.

## Final checks and submission

Test the actual Windows application from a clean extracted copy. Check valid and invalid inputs, resize behavior, export/import, and navigation. Also test repeated X values and constant Y values in regression. The independent teammate review candidate addresses those existing edge cases and must be reviewed and merged by the module owner before claiming they are fixed in master.

The final Canvas ZIP must contain the complete solution, source, PDF report, and running instructions. The report and project registration are separate deliverables. Acknowledge AI assistance and each member's actual contribution.
