# Statistics Calculator

A C# WPF project for .NET Assignment 2.

## Run

Requires Windows and the .NET 10 SDK.

Open `WpfApp.slnx` in a compatible version of Visual Studio, set `WpfApp` as the startup project, and run.

Alternatively, run these commands from the solution folder:

```powershell
dotnet build .\WpfApp.slnx -m:1
dotnet run --project .\WpfApp\WpfApp.csproj
```

## Demo account

- Username: `assignment2`
- Password: `1111`

The application creates `Users.db` and adds the demo account automatically if needed. Login uses Entity Framework, SQLite and a LINQ query. The demo password is stored as plain text.

## Features

- Main menu with a two-number calculator.
- Descriptive statistics and charts.
- Correlation and linear regression.
- Binomial probability calculations and a distribution chart.
- CSV import, pasted input, preview, export and clear.

## CSV input

Use one numeric column, with one number per row and a decimal point for decimals. Select **First row is a header** when the first row contains a heading.

Import a CSV file, or paste CSV text into the input box and click **Load pasted CSV**. Export saves the numbers shown in the preview.

## Tests

Close the application, then run:

```powershell
dotnet test .\NUnitTest\NUnitTest.csproj
```

The tests include statistics tests and a binomial probability test.

## Contributions

- **Chang Wang:** descriptive statistics, regression, and the original user model, database and login foundation.
- **Yongjian Zhou:** main-menu layout and quick calculator, binomial calculations and chart, CSV workspace, a binomial NUnit test, and simplified demo login.