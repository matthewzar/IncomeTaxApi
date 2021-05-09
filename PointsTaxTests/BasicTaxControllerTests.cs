using NUnit.Framework;
using PointsTaxAPI.Models.TaxData;
using PointsTaxAPI.Services.IncomeTaxCalculators;
using System.Collections.Generic;

namespace PointsTaxTests
{
    public class BasicTaxControllerTests
    {
        // TODO add multiple tests that test the controller.
        // Because the controller is decoupled from almost everything else you can mock all its dependencies and see how it works under any conditions

        [Test]
        public void GivenSingleBracketIncome_ExpectSimplePercentageResult()
        {
            // Arrange
            // // Create Mock ITaxBracketGetter
            // // Create Mock IIncomeTaxCalculator
            // // sut = IncomeTaxController(mockLogger, mockGetter, mockCalculator)

            // Act

            // Assert
            Assert.Warn("Incomplete test suite.");
        }
    }
}
