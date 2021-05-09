using NUnit.Framework;
using PointsTaxAPI.Models.TaxData;
using PointsTaxAPI.Services.IncomeTaxCalculators;
using System.Collections.Generic;

namespace PointsTaxTests
{
    public class BasicTaxCalculatorModelTests
    {
        // TODO - include tests that would deal with bad income-bracket data. For example:
        // - Missing upper bounds.
        // - Missing lower bounds.
        // - Brackets that don't line up (eg [[0 to 10], [12 to 100], [200+]]
        // - Min and max ranges that have been reversed (eg [min = 1000, max = 10])
        // Also need tests for current raised exceptions.

        [TestCase(100, 0.1, 10)]
        [TestCase(80,  1.0, 80)]
        [TestCase(55,  0.5, 27.5)]
        [TestCase(10,  0,   0)]
        public void GivenSingleBracketIncome_ExpectSimplePercentageResult(double income, double taxRate, double expectedTax)
        {
            // Arrange
            var taxBrackets = new TaxBracketCollection();
            taxBrackets.Brackets = new List<TaxBracket>
            { 
                TaxBracket.BuildTaxBracket(0, 100, taxRate, out bool isValid)
            };
            var sut = new PointIncomeTaxCalculator();

            // Act
            var result = sut.CalculateIncomeTax(income, taxBrackets);

            // Assert
            Assert.AreEqual(expectedTax, result);
        }

        [TestCase(100, 0.1, 10)]
        [TestCase(80, 1.0, 80)]
        [TestCase(55, 0.5, 27.5)]
        [TestCase(10, 0, 0)]
        public void GivenTwoBracketsAndLowerOnlyIncome_ExpectLowerOnlyRateResult(double income, double taxRate, double expectedTax)
        {
            // Arrange
            var taxBrackets = new TaxBracketCollection();
            taxBrackets.Brackets = new List<TaxBracket>
            {
                TaxBracket.BuildTaxBracket(0, 100, taxRate, out bool isValid1),
                TaxBracket.BuildTaxBracket(100, uint.MaxValue, taxRate*2, out bool isValid2)
            };
            var sut = new PointIncomeTaxCalculator();

            // Act
            var result = sut.CalculateIncomeTax(income, taxBrackets);

            // Assert
            Assert.AreEqual(expectedTax, result);
        }

        
        [TestCase(150, 0.1, 0.5, 35)] // 100*0.1 + 50*0.5
        [TestCase(200, 0.5, 1, 150)] // 100*0.5 + 100*1.0
        public void GivenTwoBracketsAndUpperOnlyIncome_ExpectUpperAndLowerRateMixResult(double income, double lowerRate, double upperRate, double expectedTax)
        {
            // Arrange
            var taxBrackets = new TaxBracketCollection();
            taxBrackets.Brackets = new List<TaxBracket>
            {
                TaxBracket.BuildTaxBracket(0, 100, lowerRate, out bool isValid1),
                TaxBracket.BuildTaxBracket(100, uint.MaxValue, upperRate, out bool isValid2)
            };
            var sut = new PointIncomeTaxCalculator();

            // Act
            var result = sut.CalculateIncomeTax(income, taxBrackets);

            // Assert
            Assert.AreEqual(expectedTax, result);
        }

        [TestCase(1500)]
        [TestCase(2000)]
        [TestCase(123456)]
        public void GivenMultipleBrackets_ExpectAllUsed(double income)
        {
            // Arrange
            var taxBrackets = new TaxBracketCollection();
            taxBrackets.Brackets = new List<TaxBracket>
            {
                TaxBracket.BuildTaxBracket(0, 50, 0.0, out bool isValid1),
                TaxBracket.BuildTaxBracket(50, 1000, 0.5, out bool isValid2),
                TaxBracket.BuildTaxBracket(1000, uint.MaxValue, 0.99, out bool isValid3)
            };
            var expected = 50 * 0 + 
                            (1000 - 50) * 0.5 + 
                            (income - 1000) * 0.99;

            var sut = new PointIncomeTaxCalculator();

            // Act
            var result = sut.CalculateIncomeTax(income, taxBrackets);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}