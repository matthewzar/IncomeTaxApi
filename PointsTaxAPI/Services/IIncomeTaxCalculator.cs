using PointsTaxAPI.Models.TaxData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsTaxAPI.Services
{
    /// <summary>
    /// Any income tax calculator.
    /// </summary>
    public interface IIncomeTaxCalculator
    {
        public double CalculateIncomeTax(double income, TaxBracketCollection taxBrackets);
    }
}
