using PointsTaxAPI.Models.TaxData;
using Refit;
using System.Threading.Tasks;

namespace PointsTaxAPI.Services
{
    /// <summary>
    /// A wrapper for all queries that get sent to the tax bracket reader.
    /// This could represent a DB, but is mostly used by Refit to automate API queries.
    /// </summary>
    public interface ITaxBracketGetter
    {
        [Get("/tax-calculator/brackets/{tax_year}")]
        Task<ApiResponse<TaxBracketCollection>> GetTaxData(uint tax_year);
    }
}
