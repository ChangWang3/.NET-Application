using System;
using System.Globalization;
using System.Windows;

namespace WpfApp
{
    public partial class ProbabilityWindow : Window
    {
        public ProbabilityWindow()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";
            ResultTextBox.Clear();
            DistributionPlot.Plot.Clear();
            DistributionPlot.Plot.Title("");
            DistributionPlot.Refresh();

            try
            {
                // read numbers from the three input boxes.
                if (!int.TryParse(TrialsTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int trials))
                {
                    throw new ArgumentException("Enter a whole number for the number of trials.");
                }

                if (!int.TryParse(SuccessesTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int successes))
                {
                    throw new ArgumentException("Enter a whole number for the number of successes.");
                }

                if (!double.TryParse(ProbabilityTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double successProbability))
                {
                    throw new ArgumentException("Enter a probability from 0 to 1 using a decimal point, for example 0.5.");
                }

                // reuse the existing calculations (from StaticticsCalculator file) and their range validation.
                double result;
                string description;
                if (CalculationTypeComboBox.SelectedIndex == 0)
                {
                    result = StatisticsCalculator.BinomialProbability(trials, successes, successProbability);
                    description = $"Exactly {successes} successes";
                }
                else if (CalculationTypeComboBox.SelectedIndex == 1)
                {
                    result = StatisticsCalculator.BinomialAtMostProbability(trials, successes, successProbability);
                    description = $"At most {successes} successes";
                }
                else if (CalculationTypeComboBox.SelectedIndex == 2)
                {
                    result = StatisticsCalculator.BinomialAtLeastProbability(trials, successes, successProbability);
                    description = $"At least {successes} successes";
                }
                else
                {
                    throw new ArgumentException("Choose a calculation type.");
                }

                // show both the 0-to-1 probability & its percentage; General formatting keeps very small probabilities visible
                string probabilityText = result.ToString("G10", CultureInfo.InvariantCulture);
                string percentageText = (result * 100).ToString("G10", CultureInfo.InvariantCulture);
                ResultTextBox.Text = description + "\n\nProbability: " + probabilityText
                    + "\nPercentage: " + percentageText + "%";
                DrawDistribution(trials, successProbability);
            }
            catch (ArgumentException exception)
            {
                // no old result remains on screen
                ErrorText.Text = exception.Message;
            }
        }

        private void DrawDistribution(int trials, double successProbability)
        {
            // k is the number of successes, PS: P(X = k)
            var probabilities = StatisticsCalculator.BinomialDistribution(trials, successProbability);
            double[] successCounts = new double[probabilities.Count];
            for (int k = 0; k < successCounts.Length; k++)
            {
                successCounts[k] = k;
            }

            DistributionPlot.Plot.Add.Bars(successCounts, probabilities.ToArray());
            DistributionPlot.Plot.Title($"Binomial distribution (n = {trials}, p = {successProbability.ToString("G4", CultureInfo.InvariantCulture)})");
            DistributionPlot.Plot.XLabel("Successes (k)");
            DistributionPlot.Plot.YLabel("Probability P(X = k)");
            DistributionPlot.Plot.Axes.Margins(bottom: 0);
            DistributionPlot.Plot.Axes.AutoScale();
            DistributionPlot.Refresh();
        }
    }
}
