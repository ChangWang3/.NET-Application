# Eugene's second feature: CSV Data Workspace

## What the feature does

Import a CSV containing **one numeric column**, preview its values, optionally keep
only positive numbers or change their order, and export the displayed numbers.
You can also paste CSV text directly and copy the displayed values as a
comma-separated list for the team's descriptive statistics input box.

This feature does not require a new probability formula, a database history table,
machine learning, or an additional NuGet package.

Example input:

```csv
Value
-5
0
2.5
7
10
```

The `Value` row is a **header**, meaning the name of the column rather than a number.
Select **First row is a header** for this example. Turn that option off when the
first row is already a number. The application follows your selection; it does not
guess whether a header exists.

## Files and ownership

| File | Purpose |
| --- | --- |
| `WpfApp/StatisticsCalculator.cs` | Added CSV parsing, export, and positive-value filtering methods; every existing method is retained. |
| `WpfApp/ICsvReader.cs` | Interface declaring how to read CSV text. |
| `WpfApp/ICsvWriter.cs` | Interface declaring how to create CSV text. |
| `WpfApp/CsvDataService.cs` | Implements both contracts and delegates shared logic to `StatisticsCalculator`. |
| `WpfApp/CsvWorkspace.xaml` | Eugene's resizable CSV window. |
| `WpfApp/CsvWorkspace.xaml.cs` | File dialogs, button events, preview updates, and user-facing error messages. |
| `tools/CsvPreview/` | Optional launcher to try the window independently. |

No existing main menu, login, descriptive statistics, regression, project, or
solution file has been changed. The probability methods are also retained.
Base version: GitHub master commit `31b52c1e2fd165b774bef82dc9c0c8e8c2a8e0cb`.

## Apply and try it on Windows

Download `CSV_Workspace_Only.patch` into Downloads. Close the running application,
then run these commands in PowerShell, **one line at a time**:

```powershell
Set-Location "C:\Users\13502\NET-Application-Probability"
git status --short
```

If that command prints changes, keep them and resolve the working tree before
switching branches. Do not discard files to make these instructions work.
If it prints nothing, continue:

```powershell
git switch master
git pull --ff-only origin master
git switch -c feature/eugene-csv-workspace
Test-Path "$env:USERPROFILE\Downloads\CSV_Workspace_Only.patch"
git apply --check "$env:USERPROFILE\Downloads\CSV_Workspace_Only.patch"
git apply "$env:USERPROFILE\Downloads\CSV_Workspace_Only.patch"
git status --short
dotnet build .\WpfApp\WpfApp.csproj
dotnet run --project .\tools\CsvPreview\CsvPreview.csproj
```

`Test-Path` must print `True`, and `git apply --check` must finish without errors.
If either check fails, stop before applying. The preview launcher opens only the
CSV window; the team's normal startup code remains unchanged. .NET 10 SDK and
Windows are required for the WPF UI. The project's existing SkiaSharp NU1701
warning can still appear; this feature does not change that dependency.

### Two-minute demo

1. Click **Load pasted CSV**. The preview should show all five sample numbers.
2. Tick **Only positive values (> 0)**. The preview should show `2.5`, `7`, `10`,
   with the count showing `5 imported / 3 displayed`.
3. Choose **Largest first**. The preview becomes `10`, `7`, `2.5`.
4. Click **Export displayed CSV...**, save, and open the file. It should contain
   the three displayed numbers, with a `Value` header when that export option is on.
5. Turn off the positive filter, select Original order, and import the exported
   file with the correct header option. The values should load successfully.
6. Replace the source text with `Value`, `1`, and `hello` on separate lines.
   Load it. A clear error should appear and the previous dataset should stay intact.
7. Resize the window and try **Copy displayed numbers** and **Clear**.

Preview sorting does not alter the imported list. Export and copy both use the
currently displayed values, so the filter affects what gets saved or copied.
Check the count before exporting.

## Connect to the main menu: teammate-owned integration

The CSV window exists in the main WPF project and compiles with it. The normal
main menu does **not** open it yet, because the existing teammate-owned UI files
have deliberately been left unchanged. The teammate can add a button in their
`MainWindow.xaml`, using the same pattern as the other feature buttons:

```xml
<Button Content="CSV Data Workspace" HorizontalAlignment="Center"
        VerticalAlignment="Top" Width="160" Height="30" Margin="0,200,0,0"
        Click="CsvWorkspaceBtn_Click"/>
```

Inside their `MainWindow` class, add this handler:

```csharp
private void CsvWorkspaceBtn_Click(object sender, RoutedEventArgs e)
{
    CsvWorkspace workspace = new CsvWorkspace();
    workspace.Owner = this;
    workspace.ShowDialog();
}
```

Closing the CSV window returns to the existing main menu. The independent preview
launcher is a development tool, not a replacement for this final integration.

### Commit and send for review after checking the Windows preview

Stage only the feature files, then check the list before committing:

```powershell
git add WpfApp/StatisticsCalculator.cs WpfApp/ICsvReader.cs WpfApp/ICsvWriter.cs WpfApp/CsvDataService.cs WpfApp/CsvWorkspace.xaml WpfApp/CsvWorkspace.xaml.cs CSV_Workspace_README.md tools/CsvPreview/CsvPreview.csproj tools/CsvPreview/Program.cs
git diff --cached --stat
git commit -m "Add simple CSV data workspace"
git push -u origin feature/eugene-csv-workspace
```

Then open [the comparison page](https://github.com/ChangWang3/.NET-Application/compare/master...feature/eugene-csv-workspace?expand=1)
to create a pull request. The existing source file changed should be only
`StatisticsCalculator.cs`, with additions and no deletions; the other eight files
are new. The patch has not been pushed or merged on GitHub by applying it locally.

## How this meets the code requirements

| Requirement | Actual example |
| --- | --- |
| Two interfaces | `ICsvReader` and `ICsvWriter`, implemented by `CsvDataService` and used as field types in the window. |
| Useful method overloading | `ParseCsv(string)` handles headerless data; `ParseCsv(string, bool)` supports an explicit header option. `ToCsv` also has two overloads. |
| Anonymous method with LINQ / lambda | `numbers.Where(value => value > 0).ToList()` implements the positive filter. |
| Generic collection | `List<double>` holds imported and displayed numbers. |
| Separation of responsibilities | The window handles interaction, interfaces define contracts, and `StatisticsCalculator` contains reusable data logic. |
| UI variety | Buttons, headings, text boxes, checkboxes, a dropdown, a data grid, and file/error dialogs have useful roles. |

The assignment specification explicitly allows method overloading as an example
of polymorphism. In this example the compiler selects an overload using its
arguments; this is different from runtime overriding.
This table identifies implementations, not a promise of full marks. The group
still needs NUnit tests, the final report, and four distinct functional screens.
The already merged probability calculations still need their own UI connection.

## Beginner explanation / presentation notes

**CSV 是什么？** 这是一种纯文本表格格式。这里我们只用一列数字，每行一个数；
第一行可以是列名 `Value`。界面会明确说明这个范围，多列 CSV 会被拒绝。

**数据怎样流动？** 界面取得文件或输入框中的文本 → `ICsvReader.Read` →
`CsvDataService` → `StatisticsCalculator.ParseCsv` → `List<double>` → 表格预览。
导出时顺序是：显示中的数字列表 → `ICsvWriter.Write` → `ToCsv` → 保存文件。

**Interface 是什么？** 可以理解为约定。`ICsvReader` 约定有一个读入方法，
`ICsvWriter` 约定有一个写出方法。`CsvDataService` 实现这些约定。
窗口通过接口调用服务，因此读入与写出的职责很清楚。

**Overloading 是什么？** 两个方法名字相同，但参数不同。
`ParseCsv(text)` 默认没有标题行；`ParseCsv(text, true)` 会跳过第一条记录。
一个版本调用另一个版本，避免复制解析代码。

**Lambda 和 LINQ 是什么？** `value => value > 0` 是一个没有名字的小函数，
意思是“给我一个数字，判断它是否大于零”。`Where` 保留判断结果为真的数字，
`ToList` 把结果变成新的列表。原来的导入数据没有被删除，取消筛选就能恢复。

**为什么拆开界面与共享方法？** 文件选择、按钮点击属于窗口；解析、验证和
生成 CSV 属于共享逻辑。同学写 NUnit 时可以直接测试共享方法，不需要点击界面。

Short English demo script:

> My second feature is a CSV data workspace. It reads a single column of numbers,
> previews them, and exports the displayed values. I use two interfaces to separate
> reading and writing. The parsing method is overloaded to support files with or
> without a header. A LINQ lambda keeps positive values in a new list, so the
> original dataset remains unchanged. Invalid input shows an error without
> replacing the previous data.

## Validation and limits

- Development checks: .NET 10 compilation of the main WPF project and preview
  launcher passed with zero errors. The pre-existing NU1701 package warning
  remains. A separate console verification passed 1,057 assertions, including
  exact round trips of 506 finite numeric values both with and without a header,
  malformed input, culture independence, and import-size boundaries. These checks
  are not the teammate's required NUnit tests.
- Numeric CSV only, one column, using a decimal point; no decimal commas or
  thousands separators. Quoted numeric fields are accepted.
- UTF-8 file import/export; blank rows are ignored, and a UTF-8 BOM is supported.
- Non-numeric values, malformed quotes, NaN, infinity, and empty/header-only input
  produce errors. The import accepts at most 10,000 numbers; the file picker also
  limits file size to 2 MB to keep this small desktop workspace responsive.
- Export uses invariant formatting and round-trip precision so the numeric data
  can be read again without a change of value.
- The development environment can compile WPF but cannot run a Windows window.
  The Windows demo checklist above is still required before the presentation.
- NUnit coverage is assigned to the teammate. Suggested cases: header/no header,
  negative and decimal values, invalid input, positive filtering without mutation,
  and an export/import round trip.

## Acknowledgments and reference

AI assistance was used to draft this feature, suggest an understandable scope,
check requirements, and help validate the implementation. Eugene should review
and learn the code before presenting it, and describe the actual AI use in the
assignment report.

The implementation uses .NET's `TextFieldParser` for CSV quoting and record
parsing: [Microsoft Learn — TextFieldParser](https://learn.microsoft.com/dotnet/api/microsoft.visualbasic.fileio.textfieldparser).
