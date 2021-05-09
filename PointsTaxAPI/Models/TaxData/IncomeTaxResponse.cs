using System;

namespace PointsTaxAPI.Models.TaxData
{
    /// <summary>
    /// A response body sent to any IncomeTax.Get() queries.
    /// TotalTax should be null if Success is false.
    /// </summary>
    public class IncomeTaxResponse
    {
        public double? TotalTax { get; set; }

        public string Message { get; set; }

        //public bool Success { get; set; }
    }
}
