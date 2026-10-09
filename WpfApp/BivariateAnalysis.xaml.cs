using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for BivariateAnalysis.xaml
    /// </summary>
    public partial class BivariateAnalysis : Window
    {
        public ObservableCollection<DataPoint> Points { get; } = new ObservableCollection<DataPoint>();

        public BivariateAnalysis()
        {
            InitializeComponent();

            // Sample data points
            Points.Add(new DataPoint { X = 1, Y = 107.3 });
            Points.Add(new DataPoint { X = 2, Y = 101.8 });
            Points.Add(new DataPoint { X = 3, Y = 100.2 });
            Points.Add(new DataPoint { X = 4, Y = 87.6 });
            Points.Add(new DataPoint { X = 5, Y = 63.5 });
            Points.Add(new DataPoint { X = 6, Y = 47.1 });

            //Points.Add(new DataPoint { X = 1, Y = 3 });
            //Points.Add(new DataPoint { X = 2, Y = 5 });
            //Points.Add(new DataPoint { X = 3, Y = 9 });
            //Points.Add(new DataPoint { X = 4, Y = 8 });
            //Points.Add(new DataPoint { X = 5, Y = 11 });
            //Points.Add(new DataPoint { X = 6, Y = 14 });

            this.DataContext = this;
        }

        private void BtnAddRow_Click(object sender, RoutedEventArgs e)
        {
            Points.Add(new DataPoint
            {
                X = 0,
                Y = 0
            });
        }

        private void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItem is DataPoint point)
            {
                Points.Remove(point);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Points.Clear();
        }

        private void Plot_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            DataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            double[] x = Points.Select(p => p.X).ToArray();
            double[] y = Points.Select(p => p.Y).ToArray();

            ScatterPlot.Plot.Clear();

            if (x.Length < 1 || y.Length < 1)
            {
                MessageBox.Show("Please enter data");
                return;
            }

            var scatter = ScatterPlot.Plot.Add.ScatterPoints(x, y);
            scatter.MarkerSize = 10;
            
            double xMax = x.Max();
            double yMax = y.Max();

            // Scale changes if the max x value is less than one third of the max y value or the
            // other way around else the scale is 1 to 1, adjusted to whichever max value is larger
            if (xMax < yMax / 3 || yMax < xMax / 3)
            {
                ScatterPlot.Plot.Axes.SetLimits(0, xMax + 3, 0, yMax + 3);
            } else
            {
                if (xMax > yMax)
                {
                    ScatterPlot.Plot.Axes.SetLimits(0, xMax + 1, 0, xMax + 1);
                }
                else
                {
                    ScatterPlot.Plot.Axes.SetLimits(0, yMax + 1, 0, yMax + 1);
                }
            }
            
            if (x.Length < 2 && y.Length < 2)
            {
                MessageBox.Show("Enter at least two points for analysis");
            } else
            {
                ShowLineOfBestFit(x, y, xMax, yMax);
                if (ShowEquation.IsChecked == true)
                {
                    ShowLabels(x, y, xMax, yMax);
                }
            }
            ScatterPlot.Plot.Title("Scatter Plot");
            ScatterPlot.Plot.XLabel("X");
            ScatterPlot.Plot.YLabel("Y");
            ScatterPlot.Refresh();
        }

        private void ShowLineOfBestFit(double[] x, double[] y, double xMax, double yMax)
        {
            var (slope, intercept) = StatisticsCalculator.LeastSquare(x, y);
            slope = Math.Round(slope, 2);
            intercept = Math.Round(intercept, 2);
            // Starts at x = 0, y = intercept, and ends at the max x value and the y value according to
            // slope * xMax + intercept
            ScatterPlot.Plot.Add.Line(0, intercept, xMax, slope * xMax + intercept).LineWidth = 3;
        }

        private void ShowLabels(double[] x, double[] y, double xMax, double yMax)
        {
            double correlation = StatisticsCalculator.CorrelationCoefficient(x, y);
            var (slope, intercept) = StatisticsCalculator.LeastSquare(x, y);

            slope = Math.Round(slope, 2);
            intercept = Math.Round(intercept, 2);
            string label = $"y = {slope}x";

            if (intercept >= 0)
            {
                label += $" + {intercept}";
            }
            else
            {
                label += $" - {Math.Abs(intercept)}";
            }
            label += $"\nCorrelation coefficient: {Math.Round(correlation, 3)}";
            double xMiddle = StatisticsCalculator.Median(x);
            ScatterPlot.Plot.Add.Text(label, xMiddle, slope * xMiddle + intercept).LabelFontSize = 20;
        }

        private void DataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }
    }    
}
