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

            try
            {
                // Read numbers from the three input boxes.
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

                // Reuse the existing calculations and their range validation.
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

                // Show both the 0-to-1 probability and its percentage.
                // General formatting keeps very small probabilities visible.
                string probabilityText = result.ToString("G10", CultureInfo.InvariantCulture);
                string percentageText = (result * 100).ToString("G10", CultureInfo.InvariantCulture);
                ResultTextBox.Text = description + "\n\nProbability: " + probabilityText
                    + "\nPercentage: " + percentageText + "%";
            }
            catch (ArgumentException exception)
            {
                // Invalid input is reported inline; no old result remains on screen.
                ErrorText.Text = exception.Message;
            }
        }
    }
}
