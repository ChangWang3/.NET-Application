using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace WpfApp
{
    // both interfaces are actually used by the CSV window
    // CSV parsing, exporting, and filtering
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

        // CSV workspace: 1 numeric column, 1 optional header
        // overloading: this method reads a CSV that has no header
        public static List<double> ParseCsv(string csvText)
        {
            return ParseCsv(csvText, false);
        }

        // This method lets the caller choose whether to skip a header
        public static List<double> ParseCsv(string csvText, bool hasHeader)
        {
            ArgumentNullException.ThrowIfNull(csvText); 
            List<double> numbers = new List<double>(); //create list to store valid numbers
            using StringReader reader = new StringReader(csvText.TrimStart('\uFEFF')); //read in contents from the string, and clear byte order mark at the beginning
            using TextFieldParser parser = new TextFieldParser(reader); //create .Net tool, read in contents from reader, recognize csv elements
            parser.SetDelimiters(","); //allow comma
            parser.HasFieldsEnclosedInQuotes = true; //allow quotes like "2.7"
            parser.TrimWhiteSpace = true; //clear the space before the numbers
            int recordNumber = 0; //how many csv had been read

            try
            {
                while (!parser.EndOfData) 
                {
                    string[]? fields = parser.ReadFields();
                    if (fields == null) // no contents
                    {
                        break;
                    }

                    recordNumber++;
                    if (fields.Length != 1) // can only use 1 column
                    {
                        throw new FormatException($"Record {recordNumber}: use one numeric column, one number per row.");
                    }

                    if (hasHeader && recordNumber == 1) // have header or not
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
            catch (MalformedLineException exception) // when parser.ReadFields() catch invalid contents
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

            // R preserves the numeric value when the exported CSV is read again (round trip format)
            string rows = string.Join("\r\n", numbers.Select(value => value.ToString("R", CultureInfo.InvariantCulture)));
            return (includeHeader ? "Value\r\n" : "") + rows + "\r\n";
        }
    }
}
