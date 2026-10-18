using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace WpfApp
{
    public static class StatisticsCalculator
    {
        public static double Mean(List<double> data)
        {
            double sum = 0;
            for (int i = 0; i < data.Count; i++)
            {
                sum += data[i];
            }
            return sum / data.Count;
        }

        public static double Median(List<double> data) 
        {
            if (data.Count % 2 != 0)
            {
                return data[(int)Math.Floor((double)data.Count / 2)];
            }
            else
            {
                return (data[(data.Count) / 2] + data[(data.Count - 1) / 2]) / 2;
            }
        }

        public static double Median(double[] data)
        {
            if (data.Length % 2 != 0)
            {
                return data[(int)Math.Floor((double)data.Length / 2)];
            }
            else
            {
                return (data[(data.Length) / 2] + data[(data.Length - 1) / 2]) / 2;
            }
        }

        public static double PopulationStandardDeviation(List<double> data, double mean)
        {
            if (data.Count < 2)
            {
                return -1;
            }
            double squaredDiffSum = 0;

            for (int i = 0; i < data.Count; i++)
            {
                double diff = data[i] - mean;
                squaredDiffSum += diff * diff;
            }

            return Math.Sqrt(squaredDiffSum / data.Count);
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

        public static List<double> Outliers(List<double> data, double upperQuartile, double lowerQuartile) 
        {
            List<double> outliers = new List<double>();
            double interquartileRange = upperQuartile - lowerQuartile;
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

        // Finding Pearson's correlation coefficient
        public static double CorrelationCoefficient(double[] x, double[] y)
        {
            double xSum = 0;
            double ySum = 0;
            double xySum = 0;
            double xSquaredSum = 0;
            double ySquaredSum = 0;

            for (int i  = 0; i < x.Length; i++)
            {
                xSum += x[i];
                ySum += y[i];
                xySum += x[i] * y[i];
                xSquaredSum += x[i] * x[i];
                ySquaredSum += y[i] * y[i];
            }
            return (x.Length * xySum - xSum * ySum) / Math.Sqrt(((x.Length * xSquaredSum) - 
                Math.Pow(xSum, 2)) * (x.Length * ySquaredSum - Math.Pow(ySum, 2)));
        }

        // Uses least squares to find the line of best fit, the slope and intercept are calculated together
        public static (double slope, double intercept) LeastSquares(double[] x, double[] y)
        {
            double xSum = 0;
            double ySum = 0;
            double xySum = 0;
            double xSquaredSum = 0;
            int length = x.Length;

            for (int i = 0; i < x.Length; i++)
            {
                xSum += x[i];
                ySum += y[i];
                xySum += x[i] * y[i];
                xSquaredSum += x[i] * x[i];
            }
            double slope = (length * xySum - xSum * ySum) / (length * xSquaredSum - Math.Pow(xSum, 2));
            double intercept = (ySum - slope * xSum) / length;
            return (slope, intercept);
        }

        // binomial distribution feature
        // each trial is independent and has the same chance of success,trials = n, successes = k, successProbability = p (from 0 to 1)
        public static double BinomialProbability(int trials, int successes, double successProbability)
        {
            ValidateBinomialInput(trials, successes, successProbability);

            // P(X = k) = C(n, k) * p^k * (1 - p)^(n - k), exactly k successes
            // calculate C(n,k) and p^k first, break them down not using factorials
            double probability = 1;
            for (int i = 1; i <= successes; i++)
            {
                probability *= (trials - i + 1) / (double)i;
                probability *= successProbability;
            }
            // multiply the first part with (1-p)^(n-k)
            for (int i = 1; i <= trials - successes; i++)
            {
                probability *= 1 - successProbability;
            }

            return Math.Clamp(probability, 0, 1);
        }

        // P(X <= k): add the probabilities for 0 to k successes, at most k successes
        public static double BinomialAtMostProbability(int trials, int successes, double successProbability)
        {
            ValidateBinomialInput(trials, successes, successProbability);

            double total = 0;
            for (int k = 0; k <= successes; k++)
            {
                total += BinomialProbability(trials, k, successProbability);
            }

            return Math.Clamp(total, 0, 1);
        }

        // P(X >= k): add the probabilities for k to n successes, at least k successes
        public static double BinomialAtLeastProbability(int trials, int successes, double successProbability)
        {
            ValidateBinomialInput(trials, successes, successProbability);

            double total = 0;
            for (int k = successes; k <= trials; k++)
            {
                total += BinomialProbability(trials, k, successProbability);
            }

            return Math.Clamp(total, 0, 1);
        }

        // the list index is the k, the value is P(X = k), return back to DrawDistribution method so it can draw all the pillars for k
        // offer the probabilities for the chart from k = 0 to n
        public static List<double> BinomialDistribution(int trials, double successProbability)
        {
            ValidateBinomialInput(trials, 0, successProbability); // use 0 simply to input and validate with the other 2 numbers

            List<double> probabilities = new List<double>();
            for (int k = 0; k <= trials; k++)
            {
                probabilities.Add(BinomialProbability(trials, k, successProbability));
            }

            return probabilities;
        }

        private static void ValidateBinomialInput(int trials, int successes, double successProbability)
        {
            // Limit n to 1000 so the combination calculation fits in a double
            if (trials < 0 || trials > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(trials), "Trials must be between 0 and 1000.");
            }

            if (successes < 0 || successes > trials)
            {
                throw new ArgumentOutOfRangeException(nameof(successes), "Successes must be between 0 and the number of trials.");
            }

            if (!double.IsFinite(successProbability) || successProbability < 0 || successProbability > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(successProbability), "Success probability must be between 0 and 1.");
            }
        }
    }
}
