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

        // Probability feature: binomial distribution.
        // Each trial is independent and has the same chance of success.
        // trials = n, successes = k, successProbability = p (from 0 to 1).
        // All returned probabilities are from 0 to 1, not percentages.
        public static double BinomialProbability(int trials, int successes, double successProbability)
        {
            ValidateBinomialInput(trials, successes, successProbability);

            // P(X = k) = C(n, k) * p^k * (1 - p)^(n - k).
            // A loop calculates the combination and p^k without factorials.
            double probability = 1;
            for (int i = 1; i <= successes; i++)
            {
                probability *= (trials - i + 1) / (double)i;
                probability *= successProbability;
            }

            for (int i = 0; i < trials - successes; i++)
            {
                probability *= 1 - successProbability;
            }

            return Math.Clamp(probability, 0, 1);
        }

        // P(X <= k): add the probabilities for 0, 1, ..., k successes.
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

        // P(X >= k): add the probabilities for k, k + 1, ..., n successes.
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

        // The list index is the success count k; the value is P(X = k).
        // This supplies the probabilities for a chart from k = 0 to n.
        public static List<double> BinomialDistribution(int trials, double successProbability)
        {
            ValidateBinomialInput(trials, 0, successProbability);

            List<double> probabilities = new List<double>();
            for (int k = 0; k <= trials; k++)
            {
                probabilities.Add(BinomialProbability(trials, k, successProbability));
            }

            return probabilities;
        }

        private static void ValidateBinomialInput(int trials, int successes, double successProbability)
        {
            // Limit n to 1000 so the combination calculation fits in a double.
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
