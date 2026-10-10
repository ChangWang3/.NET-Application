using NUnit.Framework;
using WpfApp;

namespace NUnitTest
{
    [TestFixture]
    public class StatisticsCalculatorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(new double[] { 8, 3, 9, 4, 7, 8 }, 6.5)]
        [TestCase(new double[] { 1, 2 }, 1.5)]
        [TestCase(new double[] { 7, 7, 7 }, 7)]
        [TestCase(new double[] { -6, 0, 2, 8 }, 1)]
        public void MeanTest(double[] input, double expected)
        {
            List<double> data = new List<double>(input);
            double result = StatisticsCalculator.Mean(data);
            Assert.That(result , Is.EqualTo(expected));
        }

        [TestCase(new double[] { 7, 1, 9, 3, 6 }, 6)]
        [TestCase(new double[] { 11, 13, 15, 17 }, 14)]
        [TestCase(new double[] { 9, 10 }, 9.5)]
        [TestCase(new double[] { 2, 2, 2 }, 2)]
        [TestCase(new double[] { -5, -1, -3 }, -3)]
        [TestCase(new double[] { -4, 0, 4, 8 }, 2)]
        public void MedianTest(double[] data, double expected)
        {            
            data.Sort();
            double result = StatisticsCalculator.Median(data);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(new double[] { 16, 34, 29, 80, 65, 77, 91, 58, 67, 40 }, 23.49)]
        [TestCase(new double[] { 56, 74, 78, 66, 51, 68, 52 }, 9.94)]
        [TestCase(new double[] { 3, 14, 19, 21, 13, 1, 30, 18, 10, 16, 27, 25 }, 8.55)]
        public void PopulationStandardDeviationTest(double[] input, double expected)
        {
            List<double> data = new List<double>(input);
            double mean = StatisticsCalculator.Mean(data);
            double result = StatisticsCalculator.PopulationStandardDeviation(data, mean);
            result = Math.Round(result, 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(new double[] { 16, 34, 29, 80, 65, 77, 91, 58, 67, 40 }, 24.76)]
        [TestCase(new double[] { 56, 74, 78, 66, 51, 68, 52 }, 10.74)]
        [TestCase(new double[] { 3, 14, 19, 21, 13, 1, 30, 18, 10, 16, 27, 25 }, 8.93)]
        public void SampleStandardDeviationTest(double[] input, double expected)
        {
            List<double> data = new List<double>(input);
            double mean = StatisticsCalculator.Mean(data);
            double result = StatisticsCalculator.SampleStandardDeviation(data, mean);
            result = Math.Round(result, 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(new double[] {1, 6, 12, 18, 23}, 25, 6)]
        [TestCase(new double[] { 1, 6, 12, 18, 23 }, 50, 12)]
        [TestCase(new double[] { 1, 6, 12, 18, 23 }, 75, 18)]
        public void PercentileTest(double[] input, double percentile, double expected)
        {
            List<double> data = new List<double>(input);
            double result = StatisticsCalculator.Percentile(data, percentile);
            Assert.That(result, Is.EqualTo(expected));
        }

        public void OutliersTest1()
        {
            List<double> data = new List<double> { 1, 6, 12, 18, 40 };
            List<double> outliers = StatisticsCalculator.Outliers(data, 18, 6);
            Assert.That(outliers, Contains.Item(40));
        }
        public void OutliersTest2()
        {
            List<double> data = new List<double> { 1, 6, 12, 18, 23 };
            List<double> outliers = StatisticsCalculator.Outliers(data, 18, 6);
            Assert.That(outliers, Is.Empty);
        }

        [TestCase(new double[] {1, 3, 5, 7, 9, 11}, new double[] {70, 66, 54, 49, 47, 36}, -0.982)]
        [TestCase(new double[] { 22, 54, 76, 98, 115, 143 }, new double[] { 54, 65, 123, 100, 124, 234 }, 0.879)]
        public void CorrelationCoefficientTest(double[] x, double[] y, double expected)
        {
            double result = StatisticsCalculator.CorrelationCoefficient(x, y);
            result = Math.Round(result, 3);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(new double[] { 1, 3, 5, 7, 9, 11 }, new double[] { 70, 66, 54, 49, 47, 36 }, -3.31, 73.55)]
        [TestCase(new double[] { 22, 54, 76, 98, 115, 143 }, new double[] { 54, 65, 123, 100, 124, 234 }, 1.30, 6.38)]
        public void LeastSquaresTest(double[] x, double[] y, double expectedSlope, double expectedIntercept)
        {
            var (slope, intercept) = StatisticsCalculator.LeastSquare(x, y);
            slope = Math.Round(slope, 2);
            intercept = Math.Round(intercept, 2);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(slope, Is.EqualTo(expectedSlope));
                Assert.That(intercept, Is.EqualTo(expectedIntercept));
            }
        }

        [TestCase(10, 6, 0.5, 0.205)]
        [TestCase(8, 5, 0.16667, 0.004)]
        public void BinomialProbabilityTest(int trials, int successes, double successPossibility, double expected)
        {
            double result = StatisticsCalculator.BinomialProbability(trials, successes, successPossibility);
            result = Math.Round(result, 3);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
