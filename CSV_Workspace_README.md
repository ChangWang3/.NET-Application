# Eugene's CSV workspace

Open CSV Workspace from the main menu. The feature imports, previews, filters, sorts, copies, and exports one numeric column. Closing its dialog returns to the menu.

## Try it

1. Click Load pasted CSV, or import samples/example_numbers.csv with First row is a header checked.
2. The preview should contain -5, 0, 2.5, 7, 10: five numbers.
3. Check Only positive values: the preview should contain 2.5, 7, 10.
4. Choose Largest first: the preview should show 10, 7, 2.5.
5. Export and import again using the matching header option.
6. Copy displayed numbers and paste them into Descriptive Statistics.
7. Try abc or a row containing 1,2. An error should appear while the previous valid preview remains available.

For data without a header, uncheck First row is a header so the first number is included. Use a decimal point and one number per row. Blank rows, quoted numbers, and a UTF-8 BOM are supported; non-finite numbers are rejected. Limits are 10,000 numeric rows and 2 MB for a file import.

Export and Copy operate on displayed values after filtering and sorting. Export a Value header controls the output heading. Clear empties the source text and dataset.

## Code to explain

- ICsvReader defines Read and ICsvWriter defines Write.
- CsvDataService implements both and owns CSV parsing, exporting, and filtering.
- CsvWorkspace uses fields typed as the interfaces.
- ParseCsv(text) reads without a header; ParseCsv(text, hasHeader) allows a choice. ToCsv has the equivalent export overloads.
- PositiveCsvValues uses the LINQ lambda value => value > 0.
- Imported and displayed values are separate lists, preserving the original data.
- Invariant culture and R formatting preserve numbers during export and reimport.

CSV transformations are in WpfApp/CsvDataService.cs. StatisticsCalculator contains mathematical calculations. No new NuGet dependency is required.

## Verification

```powershell
dotnet test .\NUnitTest\NUnitTest.csproj
```

EugeneFeatureTests covers headers, quoted input, invalid values, round trips, both interfaces, filtering, and row limits. Test the Windows dialogs and clipboard separately using the steps above.

Independent preview:

```powershell
dotnet run --project .\tools\CsvPreview\CsvPreview.csproj
```
