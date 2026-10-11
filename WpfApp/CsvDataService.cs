using System.Collections.Generic;

namespace WpfApp
{
    // Both interfaces are actually used by the CSV window.
    // Shared data logic remains in StatisticsCalculator.
    public class CsvDataService : ICsvReader, ICsvWriter
    {
        public List<double> Read(string csvText, bool hasHeader)
        {
            return hasHeader
                ? StatisticsCalculator.ParseCsv(csvText, true)
                : StatisticsCalculator.ParseCsv(csvText);
        }

        public string Write(List<double> numbers, bool includeHeader)
        {
            return includeHeader
                ? StatisticsCalculator.ToCsv(numbers)
                : StatisticsCalculator.ToCsv(numbers, false);
        }
    }
}
