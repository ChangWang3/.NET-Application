using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfApp
{
    public partial class CsvWorkspace : Window
    {
        // 2 interfaces
        private readonly ICsvReader csvReader;
        private readonly ICsvWriter csvWriter;

        private List<double> values = new List<double>();

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
                if (new FileInfo(dialog.FileName).Length > 2_000_000) //2MB
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
                // validate first; if import failed, the previous values is kept
                values = csvReader.Read(text, ImportHeaderCheckBox.IsChecked == true);
                RefreshPreview();
                StatusText.Text = $"Loaded {values.Count} numbers from {source}. Export saves the displayed values.";
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
            PreviewGrid.ItemsSource = values.Select((value, index) => new
            {
                Row = index + 1,
                Value = value.ToString("R", CultureInfo.InvariantCulture)
            }).ToList();

            CountText.Text = $"{values.Count} numbers";
            ExportButton.IsEnabled = values.Count > 0;
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
                string text = csvWriter.Write(values, ExportHeaderCheckBox.IsChecked == true);
                File.WriteAllText(dialog.FileName, text, new UTF8Encoding(false));
                StatusText.Text = $"Exported {values.Count} displayed numbers to {Path.GetFileName(dialog.FileName)}.";
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
            {
                ShowError("Could not export the CSV. " + exception.Message);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            values.Clear();
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
