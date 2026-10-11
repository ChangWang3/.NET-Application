using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    public partial class CsvWorkspace : Window
    {
        // The window uses contracts instead of depending on one concrete class.
        private readonly ICsvReader csvReader;
        private readonly ICsvWriter csvWriter;
        private List<double> importedValues = new List<double>();
        private List<double> displayedValues = new List<double>();

        public CsvWorkspace()
        {
            CsvDataService service = new CsvDataService();
            csvReader = service;
            csvWriter = service;
            InitializeComponent();
        }

        private void ImportCsv_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "CSV files (*.csv)|*.csv", Title = "Import one numeric column" };
            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            try
            {
                if (new FileInfo(dialog.FileName).Length > 2_000_000)
                {
                    ShowError("Choose a CSV smaller than 2 MB, with at most 10,000 numbers.");
                    return;
                }

                string text = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                if (LoadCsv(text, Path.GetFileName(dialog.FileName)))
                {
                    CsvInputTextBox.Text = text;
                }
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                ShowError("Could not read the file. " + exception.Message);
            }
        }

        private void LoadPastedCsv_Click(object sender, RoutedEventArgs e)
        {
            LoadCsv(CsvInputTextBox.Text, "pasted CSV");
        }

        private bool LoadCsv(string text, string source)
        {
            try
            {
                // Validate first; a failed import leaves the previous dataset intact.
                List<double> parsedValues = csvReader.Read(text, ImportHeaderCheckBox.IsChecked == true);
                importedValues = parsedValues;
                RefreshPreview();
                StatusText.Text = $"Loaded {importedValues.Count} numbers from {source}. Export saves the displayed values.";
                return true;
            }
            catch (FormatException exception)
            {
                ShowError(exception.Message);
                return false;
            }
        }

        private void RefreshPreview()
        {
            displayedValues = PositiveOnlyCheckBox.IsChecked == true
                ? StatisticsCalculator.PositiveCsvValues(importedValues)
                : importedValues.ToList();

            if (SortComboBox.SelectedIndex == 1)
            {
                displayedValues = displayedValues.OrderBy(value => value).ToList();
            }
            else if (SortComboBox.SelectedIndex == 2)
            {
                displayedValues = displayedValues.OrderByDescending(value => value).ToList();
            }

            PreviewGrid.ItemsSource = displayedValues.Select((value, index) => new
            {
                Row = index + 1,
                Value = value.ToString("R", CultureInfo.InvariantCulture)
            }).ToList();
            CountText.Text = $"{importedValues.Count} imported / {displayedValues.Count} displayed";
            ExportButton.IsEnabled = displayedValues.Count > 0;
            CopyButton.IsEnabled = displayedValues.Count > 0;
        }

        private void PreviewOptions_Changed(object sender, RoutedEventArgs e)
        {
            if (PreviewGrid != null)
            {
                RefreshPreview();
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // XAML raises this event while the controls are still being created.
            if (PreviewGrid != null)
            {
                RefreshPreview();
            }
        }

        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv", FileName = "numbers.csv", DefaultExt = ".csv", AddExtension = true
            };
            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            try
            {
                string text = csvWriter.Write(displayedValues, ExportHeaderCheckBox.IsChecked == true);
                File.WriteAllText(dialog.FileName, text, new UTF8Encoding(false));
                StatusText.Text = $"Exported {displayedValues.Count} displayed numbers to {Path.GetFileName(dialog.FileName)}.";
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
            {
                ShowError("Could not export the CSV. " + exception.Message);
            }
        }

        private void CopyNumbers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = string.Join(",", displayedValues.Select(value => value.ToString("R", CultureInfo.InvariantCulture)));
                Clipboard.SetText(text);
                StatusText.Text = "Copied displayed numbers. Paste them into the statistics input box.";
            }
            catch (ExternalException)
            {
                ShowError("The clipboard is busy. Please try copying again.");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            importedValues.Clear();
            CsvInputTextBox.Clear();
            RefreshPreview();
            StatusText.Text = "Workspace cleared.";
        }

        private void ShowError(string message)
        {
            StatusText.Text = "Action failed. " + message;
            MessageBox.Show(this, message, "CSV workspace", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
