using ScottPlot;
using ScottPlot.Colormaps;
using ScottPlot.Plottables;
using ScottPlot.Statistics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for UnivariateAnalysis.xaml
    /// </summary>
    public partial class UnivariateAnalysis : Window
    {
        public UnivariateAnalysis()
        {
            InitializeComponent();
        }

        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void txtData_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dataText = txtData.Text;
                List<double> data = dataText.Split(',').Select(double.Parse).ToList();
                data.Sort();

                if (data.Count > 1)
                {
                    double mean = StatisticsCalculator.Mean(data);
                    double median = StatisticsCalculator.Median(data);
                    // StatisticsCalculator.Percentile(data, 50) will also work to get median
                    double upperQuartile = StatisticsCalculator.Percentile(data, 75);
                    double lowerQuartile = StatisticsCalculator.Percentile(data, 25);
                    double interquartileRange = upperQuartile - lowerQuartile;

                    txtResult.Text = "Mean: " + mean + "\nMedian: " + median + "\nCount: " +
                    data.Count + "\nMin: " + data.First() + "\nMax: " + data.Last() +
                    "\nRange: " + Math.Abs(data.Last() - data.First()) + "\nUpper Quartile: " + 
                    upperQuartile + "\nLower Quartile: " +
                    lowerQuartile + "\nInterquartile Range: " + interquartileRange;

                    List<double> outliers = StatisticsCalculator.Outliers(data, upperQuartile, 
                        lowerQuartile, interquartileRange);
                    if (outliers.Count > 0) 
                    {
                        txtResult.Text += "\nOutliers: ";
                        foreach (double n in outliers)
                        {
                            txtResult.Text += n + " ";
                        }
                    }

                    // Only one radio buttun can be checked
                    if (RadioPopulationStd.IsChecked == true)
                    {
                        double popStd = StatisticsCalculator.PopulateStandardDeviation(data, mean);
                        txtResult.Text += "\nStandard Deviation: " + popStd;
                    }
                    if (RadioSampleStd.IsChecked == true)
                    {
                        double samStd = StatisticsCalculator.SampleStandardDeviation(data, mean);
                        txtResult.Text += "\nStandard Deviation: " + samStd;
                    }
                    
                    if (DataAreIntegers(data))
                    {
                        // Histogram for discrete values
                        CreateBarPlot(data);
                    } else
                    {
                        // Histogram for decimals
                        CreateHistogram(data);
                        
                    }
                    CreateBoxPlot(data, data.First(), data.Last(), median, upperQuartile, lowerQuartile);
                }
                else
                {
                    MessageBox.Show("Enter at least 2 numbers for statistical analysis");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Input was incorrect");
            }
        }

        private bool DataAreIntegers(List<double> data)
        {
            bool allDataAreIntegers = true;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] % 1 != 0)
                {
                    allDataAreIntegers = false;
                    break;
                }
            }
            return allDataAreIntegers;
        }

        private void CreateHistogram(List<double> data)
        {
            // Clears the previous histogram
            HistogramPlot.Plot.Clear();

            var hist = ScottPlot.Statistics.Histogram.WithBinCount(count: 20, minValue: data.First() - 1, maxValue: data.Last() + 1);
            var histPlot = HistogramPlot.Plot.Add.Histogram(hist);
            histPlot.BarWidthFraction = 0.7;
            hist.AddRange(data);
            HistogramPlot.Plot.Axes.AutoScaleY();

            HistogramPlot.Plot.Title("Histogram");
            HistogramPlot.Plot.XLabel("Value");
            HistogramPlot.Plot.YLabel("Frequency");
            HistogramPlot.Plot.Axes.Margins(bottom: 0.1, top: 0.1);
            HistogramPlot.Refresh();

        }
        private void CreateBarPlot(List<double> data)
        {
            // Put equal values into the same group and put to an array
            var groups = data.GroupBy(x => x).ToArray();

            // Gets the key values of the groups
            double[] xValues = groups.Select(g => g.Key).ToArray();

            // Returns the number of times each value occurred
            double[] counts = groups.Select(g => (double)g.Count()).ToArray();

            HistogramPlot.Plot.Clear();
            var bars = HistogramPlot.Plot.Add.Bars(xValues, counts);

            HistogramPlot.Plot.Title("Bar Plot");
            HistogramPlot.Plot.XLabel("Value");
            HistogramPlot.Plot.YLabel("Frequency");

            HistogramPlot.Plot.Axes.Margins(bottom: 0.1, top: 0.1);

            HistogramPlot.Refresh();
            
        }

        private void CreateBoxPlot(List<double> data, double min, double max, double median,
            double upperQuartile, double lowerQuartile)
        {
            BoxAndWhiskersPlot.Plot.Clear();
            ScottPlot.Box box = new()
            {
                Position = 5,
                BoxMin = lowerQuartile,
                BoxMax = upperQuartile,
                WhiskerMin = min,
                WhiskerMax = max,
                BoxMiddle = median,
            };

            BoxAndWhiskersPlot.Plot.Title("Box and Whiskers Plot");
            BoxAndWhiskersPlot.Plot.Add.Box(box);
            BoxAndWhiskersPlot.Plot.Axes.SetLimits(0, 10, min - 1, max + 1);
            BoxAndWhiskersPlot.Refresh();
        }

    }
}
