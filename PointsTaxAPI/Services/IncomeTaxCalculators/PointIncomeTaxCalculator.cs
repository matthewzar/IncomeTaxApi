using PointsTaxAPI.Models.TaxData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsTaxAPI.Services.IncomeTaxCalculators
{
    public class PointIncomeTaxCalculator : IIncomeTaxCalculator
    {
        /// <summary>
        /// Calculates total income tax based on a list of tax brackets.
        /// 
        /// In the event of a missing upperbound bracket, the largest tax rate will be applied to whatever funds have not yet been taxed. Does not check for missing lower bounds, or any other malformed TaxBracket data.
        /// </summary>
        /// <param name="totalIncome">Income to calculate taxes for.</param>
        /// <param name="taxBrackets">The rates for each tax bracket.</param>
        /// <returns></returns>
        public double CalculateIncomeTax(double totalIncome, TaxBracketCollection taxBrackets)
        {
            double result = 0;
            double maxRate = 0;
            double untaxedIncome = totalIncome;

            foreach(var bracket in taxBrackets.Brackets)
            {
                maxRate = Math.Max(maxRate, bracket.Rate);
                if (bracket.Min > bracket.Max) throw new ArgumentException($"Min tax rate {bracket.Min} cannot be greater than max {bracket.Max}.");

                // Don't increase if you're outside the current bracket.
                if (totalIncome < bracket.Min) continue;

                double incomeToTax;
                // Is income over the bracket? 
                if (totalIncome >= bracket.Max)
                {
                    // Tax the full range.
                    incomeToTax = bracket.Max - bracket.Min;
                }
                else
                {
                    // Only tax what's left.
                    incomeToTax = totalIncome - bracket.Min;
                }
                result += incomeToTax * bracket.Rate;
                untaxedIncome -= incomeToTax;
            }

            if (untaxedIncome != 0)
            {
                // TODO - add logger.Warn(dangerous bracket data)
                result += untaxedIncome * maxRate;
            }

            return Math.Round(result, 2);
        }
    }
}
