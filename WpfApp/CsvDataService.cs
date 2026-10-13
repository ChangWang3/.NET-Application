using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace WpfApp
{
    // Both interfaces are actually used by the CSV window.
    // CSV parsing, exporting, and filtering belong to this service.
    public class CsvDataService : ICsvReader, ICsvWriter
    {
        public List<double> Read(string csvText, bool hasHeader)
        {
            return hasHeader
                ? ParseCsv(csvText, true)
                : ParseCsv(csvText);
        }

        public string Write(List<double> numbers, bool includeHeader)
        {
            return includeHeader
                ? ToCsv(numbers)
                : ToCsv(numbers, false);
        }

        // Eugene's CSV workspace: one numeric column, with an optional header.
        // Overloading: this version reads a CSV that has no header.
        public static List<double> ParseCsv(string csvText)
        {
            return ParseCsv(csvText, false);
        }

        // This version lets the caller explicitly choose whether to skip a header.
        public static List<double> ParseCsv(string csvText, bool hasHeader)
        {
            ArgumentNullException.ThrowIfNull(csvText);
            List<double> numbers = new List<double>();
            using StringReader reader = new StringReader(csvText.TrimStart('\uFEFF'));
            using TextFieldParser parser = new TextFieldParser(reader);
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;
            parser.TrimWhiteSpace = true;
            int recordNumber = 0;

            try
            {
                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields();
                    if (fields == null)
                    {
                        break;
                    }

                    recordNumber++;
                    if (fields.Length != 1)
                    {
                        throw new FormatException($"Record {recordNumber}: use one numeric column, one number per row.");
                    }

                    if (hasHeader && recordNumber == 1)
                    {
                        continue;
                    }

                    if (!double.TryParse(fields[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                        || !double.IsFinite(value))
                    {
                        throw new FormatException($"Record {recordNumber}: enter a finite number using a decimal point, for example 2.5.");
                    }

                    if (numbers.Count >= 10000)
                    {
                        throw new FormatException("Import at most 10,000 numbers at a time.");
                    }

                    numbers.Add(value);
                }
            }
            catch (MalformedLineException exception)
            {
                throw new FormatException($"Invalid CSV quoting near line {parser.ErrorLineNumber}.", exception);
            }

            if (numbers.Count == 0)
            {
                throw new FormatException("The CSV contains no numbers. Check the header option and enter at least one number.");
            }

            return numbers;
        }

        // The default export includes a column heading called Value.
        public static string ToCsv(List<double> numbers)
        {
            return ToCsv(numbers, true);
        }

        public static string ToCsv(List<double> numbers, bool includeHeader)
        {
            ArgumentNullException.ThrowIfNull(numbers);
            if (numbers.Count == 0 || numbers.Any(value => !double.IsFinite(value)))
            {
                throw new ArgumentException("Export requires at least one finite number and no invalid values.", nameof(numbers));
            }

            // R preserves the numeric value when the exported CSV is read again.
            string rows = string.Join("\r\n", numbers.Select(value => value.ToString("R", CultureInfo.InvariantCulture)));
            return (includeHeader ? "Value\r\n" : "") + rows + "\r\n";
        }

        public static List<double> PositiveCsvValues(List<double> numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);
            // A LINQ lambda: keep each value only when it is greater than zero.
            // ToList creates a new list, so the imported data stays unchanged.
            return numbers.Where(value => value > 0).ToList();
        }
    }
}
