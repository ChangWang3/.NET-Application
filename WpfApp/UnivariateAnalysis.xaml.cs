using System;
using System.Collections.Generic;
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
using System.Linq;

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

        private void txtNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string numbersText = txtNumbers.Text;
                List<double> numbers = numbersText.Split(',').Select(double.Parse).ToList();
                numbers.Sort();
                double sum = 0;
                for (int i = 0; i < numbers.Count; i++)
                {
                    sum += numbers[i];
                }

                double median;
                if (numbers.Count % 2 != 0 && numbers.Count > 1)
                {
                    median = numbers[numbers.Count / 2];
                } else
                {
                    median = (numbers[(numbers.Count + 1) / 2] + numbers[(numbers.Count - 1) / 2]) / 2;
                }
                double mean = sum / numbers.Count;
                txtResult.Text = "Mean: " + mean + "\nMedian: " + median + "\nCount: " + 
                    numbers.Count + "\nMin: " + numbers.First() + "\nMax: " + numbers.Last() + 
                    "\nRange: " + (numbers.Last() - numbers.First());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Input was incorrect");
            }
        }

        //private double CalculateMean(List<double> numbers)
        //{
        //    double sum = 0;
        //    for (int i = 0; i < numbers.Count; i++)
        //    {
        //        sum += numbers[i];
        //    }
        //    return sum / numbers.Count;
        //}
        //private double GetMin(List<double> numbers) 
        //{
        //    for (int i)
        //    return 0;
        //}
    }
}
