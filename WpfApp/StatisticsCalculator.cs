using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp
{
    public static class StatisticsCalculator
    {
        public static double Mean(List<double> numbers)
        {
            double sum = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                sum += numbers[i];
            }
            return sum / numbers.Count;
        }

        public static double Median(List<double> numbers) 
        {
            if (numbers.Count % 2 != 0)
            {
                return numbers[(int)Math.Floor((double)numbers.Count / 2)];
            }
            else
            {
                return (numbers[(numbers.Count + 1) / 2] + numbers[(numbers.Count - 1) / 2]) / 2;
            }
        }

        public static double PopulateStandardDeviation(List<double> numbers, double mean)
        {
            if (numbers.Count < 2)
            {
                return -1;
            }
            double squaredDiffSum = 0;

            for (int i = 0; i < numbers.Count; i++)
            {
                double diff = numbers[i] - mean;
                squaredDiffSum += diff * diff;
            }

            return Math.Sqrt(squaredDiffSum / numbers.Count);
        }

        public static double SampleStandardDeviation(List<double> numbers, double mean)
        {
            if (numbers.Count < 2)
            {
                return -1;
            }
            double squaredDiffSum = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                double diff = numbers[i] - mean;
                squaredDiffSum += diff * diff;
            }

            return Math.Sqrt(squaredDiffSum / (numbers.Count - 1));
        }

        public static double Percentile(List<double> data, double percentile)
        {
            double position = (percentile / 100) * (data.Count - 1);
            int upper = (int)Math.Ceiling(position);
            int lower = (int)Math.Floor(position);

            if (upper == lower)
            {
                return data[upper];
            }

            double fraction = position - lower;

            return data[lower] + fraction * (data[upper] - data[lower]);
        }

        public static List<double> Outliers(List<double> data, double upperQuartile, double lowerQuartile, double interquartileRange) 
        {
            List<double> outliers = new List<double>();
            double lowerBound = lowerQuartile - interquartileRange * 1.5;
            double upperBound = upperQuartile + interquartileRange * 1.5;

            foreach (double n in data)
            {
                if (n < lowerBound || n > upperBound)
                {
                    outliers.Add(n);
                }
            }
            return outliers;
        }
    }
}
