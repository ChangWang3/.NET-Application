using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WpfApp;

namespace NUnitTest;

[TestFixture]
public class EugeneFeatureTests
{
    [TestCase(4, 1, 0.5, 0.25)]
    [TestCase(4, 2, 0.5, 0.375)]
    [TestCase(0, 0, 0.5, 1)]
    [TestCase(4, 0, 0, 1)]
    [TestCase(4, 4, 1, 1)]
    public void ExactlyMatchesKnownProbabilities(int n, int k, double p, double expected)
    {
        Assert.That(StatisticsCalculator.BinomialProbability(n, k, p), Is.EqualTo(expected).Within(1e-12));
    }

    [TestCase(4, 1, 0.5, 0.3125)]
    [TestCase(4, 2, 0.5, 0.6875)]
    [TestCase(4, 4, 0.5, 1)]
    public void AtMostMatchesKnownProbabilities(int n, int k, double p, double expected)
    {
        Assert.That(StatisticsCalculator.BinomialAtMostProbability(n, k, p), Is.EqualTo(expected).Within(1e-12));
    }

    [TestCase(4, 1, 0.5, 0.9375)]
    [TestCase(4, 2, 0.5, 0.6875)]
    [TestCase(4, 0, 0.5, 1)]
    public void AtLeastMatchesKnownProbabilities(int n, int k, double p, double expected)
    {
        Assert.That(StatisticsCalculator.BinomialAtLeastProbability(n, k, p), Is.EqualTo(expected).Within(1e-12));
    }

    [TestCase(-1, 0, 0.5)]
    [TestCase(1001, 0, 0.5)]
    [TestCase(4, -1, 0.5)]
    [TestCase(4, 5, 0.5)]
    [TestCase(4, 1, -0.1)]
    [TestCase(4, 1, 1.1)]
    public void InvalidProbabilityRangesAreRejected(int n, int k, double p)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => StatisticsCalculator.BinomialProbability(n, k, p));
    }

    [TestCase(double.NaN)]
    [TestCase(double.PositiveInfinity)]
    [TestCase(double.NegativeInfinity)]
    public void NonFiniteProbabilityIsRejected(double p)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => StatisticsCalculator.BinomialProbability(4, 1, p));
    }

    [Test]
    public void DistributionForFourFairCoinTrialsMatchesAllOutcomes()
    {
        Assert.That(StatisticsCalculator.BinomialDistribution(4, 0.5),
            Is.EqualTo(new[] { 0.0625, 0.25, 0.375, 0.25, 0.0625 }).Within(1e-12));
    }

    [Test]
    public void CsvWithoutHeaderKeepsTheFirstNumber()
    {
        Assert.That(CsvDataService.ParseCsv("-5\r\n0\r\n2.5\r\n7\r\n10"),
            Is.EqualTo(new[] { -5.0, 0, 2.5, 7, 10 }));
    }

    [Test]
    public void ReaderInterfaceSupportsBomHeaderBlankRowsAndQuotedNumbers()
    {
        ICsvReader reader = new CsvDataService();
        Assert.That(reader.Read("\uFEFFValue\r\n\r\n\"2.5\"\r\n-3\r\n", true), Is.EqualTo(new[] { 2.5, -3.0 }));
    }

    [TestCase("")]
    [TestCase("Value\r\nabc")]
    [TestCase("Value\r\n1,2")]
    [TestCase("Value\r\nNaN")]
    [TestCase("Value\r\nInfinity")]
    [TestCase("Value\r\n1e999")]
    [TestCase("Value\r\n\"2")]
    public void InvalidCsvIsRejected(string text)
    {
        Assert.Throws<FormatException>(() => CsvDataService.ParseCsv(text, true));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void WriterAndReaderInterfacesPreserveNumericValues(bool header)
    {
        CsvDataService service = new CsvDataService();
        ICsvWriter writer = service;
        ICsvReader reader = service;
        List<double> numbers = new List<double> { -5, 0, 0.1, 1.2345678901234567, 1e-100 };
        Assert.That(reader.Read(writer.Write(numbers, header), header), Is.EqualTo(numbers));
    }

    [TestCase(double.NaN)]
    [TestCase(double.PositiveInfinity)]
    [TestCase(double.NegativeInfinity)]
    public void NonFiniteExportIsRejected(double value)
    {
        Assert.Throws<ArgumentException>(() => CsvDataService.ToCsv(new List<double> { value }));
    }

    [Test]
    public void PositiveFilterExcludesZeroAndPreservesOriginalData()
    {
        List<double> original = new List<double> { -5, 0, 2.5, 7, 10 };
        Assert.That(CsvDataService.PositiveCsvValues(original), Is.EqualTo(new[] { 2.5, 7, 10 }));
        Assert.That(original, Is.EqualTo(new[] { -5.0, 0, 2.5, 7, 10 }));
    }

    [Test]
    public void EmptyExportIsRejected()
    {
        Assert.Throws<ArgumentException>(() => CsvDataService.ToCsv(new List<double>()));
    }

    [Test]
    public void CsvAcceptsTenThousandNumbers()
    {
        Assert.That(CsvDataService.ParseCsv(string.Join("\n", Enumerable.Repeat("1", 10000))), Has.Count.EqualTo(10000));
    }

    [Test]
    public void CsvRejectsMoreThanTenThousandNumbers()
    {
        Assert.Throws<FormatException>(() => CsvDataService.ParseCsv(string.Join("\n", Enumerable.Repeat("1", 10001))));
    }
}
