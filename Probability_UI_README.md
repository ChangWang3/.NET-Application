# Simple binomial probability window

This patch adds a small WPF window to Eugene's existing binomial probability feature.
It uses the calculations already merged into `WpfApp/StatisticsCalculator.cs`.

## Scope

The patch contains five new files only:

- `WpfApp/ProbabilityWindow.xaml`
- `WpfApp/ProbabilityWindow.xaml.cs`
- `tools/ProbabilityPreview/ProbabilityPreview.csproj`
- `tools/ProbabilityPreview/Program.cs`
- `Probability_UI_README.md`

No existing file is changed. The shared calculations remain in
`StatisticsCalculator.cs`. The main menu remains for the teammate to connect.
The patch was prepared against master commit `d344e0974a5727814895cbd8a6d5379e80fbf37f`.
Do not reapply the old `Probability_Only.patch`; those calculations are already merged.

## Download and apply on Windows

If you downloaded `Probability_UI_Download.zip`, extract it into Downloads.
The ZIP contains `Probability_UI_Only.patch` and this README.
Ensure the patch ends up directly inside Downloads rather than a nested folder.

```powershell
Test-Path "$env:USERPROFILE\Downloads\Probability_UI_Only.patch"
Set-Location "C:\Users\13502\NET-Application-Probability"
git status --short
```

The first command should return `True`. Continue only with a clean working tree.
If this UI branch already exists, switch to it instead of creating it again;
if you already applied the patch, do not apply it twice.

```powershell
git switch master
git pull --ff-only origin master
git switch -c feature/eugene-probability-ui
git apply --check "$env:USERPROFILE\Downloads\Probability_UI_Only.patch"
git apply "$env:USERPROFILE\Downloads\Probability_UI_Only.patch"
git status --short
dotnet build .\WpfApp\WpfApp.csproj
dotnet run --project .\tools\ProbabilityPreview\ProbabilityPreview.csproj
```

`git apply --check` succeeds without printing anything. Before staging,
`git status --short` shows new, untracked files (`??`). A plain `git diff`
does not show untracked files.

The preview opens the probability window directly, without changing the main menu.
Use Visual Studio or the commands above with the .NET 10 SDK.

## Try the window

Enter `n = 4`, `k = 2`, `p = 0.5`. Click Calculate and try all three options.

| Calculation | Probability | Percentage |
| --- | --- | --- |
| Exactly 2 successes | 0.375 | 37.5% |
| At most 2 successes | 0.6875 | 68.75% |
| At least 2 successes | 0.6875 | 68.75% |

To distinguish the two cumulative options, use `n = 4`, `k = 1`, `p = 0.5`.
Exactly gives 25%, at most gives 31.25%, and at least gives 93.75%.

Also try a letter in the trials box, `k > n`, and `p = 2`.
An error should appear and the old result should disappear.
After correcting the inputs, Calculate should work again.
Try resizing the window and using the Enter key to calculate.

Close the preview, then run the existing group tests:

```powershell
dotnet test .\NUnitTest\NUnitTest.csproj
```

The existing SkiaSharp NU1701 warning is unrelated to this window.
Actual window interaction still needs to be checked on Windows.

## Commit and push after checking

```powershell
git add WpfApp/ProbabilityWindow.xaml WpfApp/ProbabilityWindow.xaml.cs tools/ProbabilityPreview/ProbabilityPreview.csproj tools/ProbabilityPreview/Program.cs Probability_UI_README.md
git diff --cached --numstat
git commit -m "Add simple binomial probability window"
git push -u origin feature/eugene-probability-ui
```

The staged diff should show five new files and zero deleted lines.
Create a GitHub pull request from this branch into master.

## Main menu integration for the teammate

The patch does not edit `MainWindow.xaml` or `MainWindow.xaml.cs`.
The teammate can add a button to the existing Grid, adjusting its position as needed:

```xml
<Button Content="Binomial Probability" HorizontalAlignment="Center"
        VerticalAlignment="Top" Width="160" Height="30" Margin="0,250,0,0"
        Click="ProbabilityBtn_Click"/>
```

Add the following handler inside the existing `MainWindow` class:

```csharp
private void ProbabilityBtn_Click(object sender, RoutedEventArgs e)
{
    ProbabilityWindow probability = new ProbabilityWindow();
    probability.Owner = this;
    probability.ShowDialog();
}
```

This integration must happen before presenting the complete application.

## Beginner explanation

- `n` is the number of trials, such as four coin flips.
- `k` is the number of successes, such as two heads.
- `p` is the chance of success in each trial. Enter `0.5` for 50%.
- Trials should be independent and use the same success probability.
- Exactly means equal to k; at most includes 0 through k; at least includes k through n.
- XAML defines the controls and layout. The C# code handles the Calculate click.
- `TryParse` checks whether the input is a number.
- The selected option calls one of the three existing shared calculation methods.
- The result is displayed as a probability and as a percentage (`result * 100`).
- Input range checking is reused from `StatisticsCalculator`; the window does not duplicate the formula.

Presentation example:

> My first feature is a binomial probability calculator. The user enters the
> number of trials, the number of successes, and the success probability.
> A dropdown selects exactly, at most, or at least. The window calls the shared
> calculation methods and shows the probability and percentage. Invalid input
> displays an error instead of keeping an old result.

Describe AI assistance with implementation, explanations and checking in the
report acknowledgments, as required by the assignment.
