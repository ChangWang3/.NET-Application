using System;
using System.Globalization;
using System.Windows;

namespace WpfApp
{
    //interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void QuickCalculate_Click(object sender, RoutedEventArgs e)
        {
            QuickResultText.Text = "";
            QuickErrorText.Text = "";

            // TryParse checks the text; IsFinite rejects NaN and infinity.
            if (!double.TryParse(FirstNumberTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double first)
                || !double.TryParse(SecondNumberTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double second)
                || !double.IsFinite(first) || !double.IsFinite(second))
            {
                QuickErrorText.Text = "Enter two finite numbers, for example -2 or 3.5.";
                return;
            }

            double result;
            if (OperationComboBox.SelectedIndex == 0)
                result = first + second;
            else if (OperationComboBox.SelectedIndex == 1)
                result = first - second;
            else if (OperationComboBox.SelectedIndex == 2)
                result = first * second;
            else if (OperationComboBox.SelectedIndex == 3)
            {
                if (second == 0)
                {
                    QuickErrorText.Text = "Cannot divide by zero.";
                    return;
                }
                result = first / second;
            }
            else
            {
                QuickErrorText.Text = "Choose an operation.";
                return;
            }

            if (!double.IsFinite(result))
            {
                QuickErrorText.Text = "The result is too large. Try smaller numbers.";
                return;
            }

            QuickResultText.Text = "Result: " + result.ToString("G10", CultureInfo.InvariantCulture); //G10 & Culture~ clears the format
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen login = new LoginScreen();
            login.Show();
            Close();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UnivariateBtn_Click(object sender, RoutedEventArgs e)
        {
            UnivariateAnalysis univariateAnalysis = new UnivariateAnalysis();
            univariateAnalysis.Show();
            Close();
        }

        private void BivariateBtn_Click(object sender, RoutedEventArgs e)
        {
            BivariateAnalysis bivariateAnalysis = new BivariateAnalysis();
            bivariateAnalysis.Show();
            Close();
        }

        private void ProbabilityBtn_Click(object sender, RoutedEventArgs e)
        {
            ProbabilityWindow probability = new ProbabilityWindow();
            probability.Owner = this;
            probability.ShowDialog();
        }

        private void CsvWorkspaceBtn_Click(object sender, RoutedEventArgs e)
        {
            CsvWorkspace csvWorkspace = new CsvWorkspace();
            csvWorkspace.Owner = this;
            csvWorkspace.ShowDialog();
        }
    }
}
