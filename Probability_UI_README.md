# Eugene's binomial probability window

Open Binomial Probability from the main menu. Closing the dialog returns to the menu. StatisticsCalculator contains the calculations; the window reads inputs and displays probability and percentage.

## Demonstration

Use n=4, k=1, p=0.5, representing four independent fair-coin trials:

- Exactly one success: 0.25, or 25%.
- At most one success: 0.3125, or 31.25%.
- At least one success: 0.9375, or 93.75%.

Try k greater than n, a negative n, text in a numeric field, and p greater than 1. An error appears inline and the old result is cleared.

## Code to explain

Calculate_Click uses TryParse to read the input boxes. The selected calculation type chooses BinomialProbability, BinomialAtMostProbability, or BinomialAtLeastProbability. The calculator validates n=0..1000, k=0..n, and finite p=0..1. The result is a probability from zero to one; the window multiplies by 100 to display a percentage.

The calculation assumes independent trials with the same success probability. At-most and at-least calculations add the probabilities for the relevant success counts. BinomialDistribution supplies a list for all counts; the simple window displays the selected result.

## Verification

Run dotnet test .\NUnitTest\NUnitTest.csproj. EugeneFeatureTests covers known probabilities, cumulative outcomes, invalid ranges, non-finite p, and a complete small distribution. Also check button behavior and resize on Windows.

Independent preview: dotnet run --project .\tools\ProbabilityPreview\ProbabilityPreview.csproj.
