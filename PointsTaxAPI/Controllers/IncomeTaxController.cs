using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PointsTaxAPI.Models.TaxData;
using PointsTaxAPI.Services;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointsTaxAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncomeTaxController : ControllerBase
    {
        private readonly IIncomeTaxCalculator _taxCalculator;
        private readonly ITaxBracketGetter _taxBracketGetter;
        private readonly ILogger<IncomeTaxController> _logger;

        public IncomeTaxController(ILogger<IncomeTaxController> logger, IIncomeTaxCalculator taxCalculator, ITaxBracketGetter taxBracketGetter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _taxCalculator = taxCalculator ?? throw new ArgumentNullException(nameof(taxCalculator));
            _taxBracketGetter = taxBracketGetter ?? throw new ArgumentNullException(nameof(taxBracketGetter));

            _logger.LogTrace("Controller created.");
        }

        /// <summary>
        /// Attempts to calculate, and return, income tax for a given year.
        /// The presence of the tax-brackets for each year is not guaranteed.
        /// 
        /// An example of how to reach this endpoint.
        /// https://localhost:44359/IncomeTax?year=2020&income=50000&retries=2
        /// </summary>
        /// <param name="income">The amount of money you want to calculate taxes for.</param>
        /// <param name="year">The year you're calculating taxes for</param>
        /// <param name="retries">An optional argument that lets you try querying the backend more than once.</param>
        /// <returns></returns>
        [EnableCors("Unrestricted")]
        [HttpGet()]
        public async Task<IncomeTaxResponse> Get(double income, uint year, uint retries = 3)
        {
            // // This is an option if the rate-api will never update. But if it may have data added, hardcoding would block new year data.
            // if (year != 2020 && year != 2019) return "Short circuit on known bad year";

            _logger.LogTrace($"Controller Tax.Get() called. Income: {income}, Year: {year}, MaxRetried: {retries}");
            try
            {
                var response = await _taxBracketGetter.GetTaxData(year);

                // If there's an error, check what error occured (and maybe try again).
                while (response.Error != null)
                {                    
                    var message = await getFirstErrorMessage(response.Error);
                    _logger.LogInformation($"Error occured with message {response.Error}. Retries remaining: {retries}");

                    // Return an error if the year can't be found, or the server has had too many retries
                    if (message.Contains("Tax brackets for the year") || retries-- <= 0)                                                                                           
                    {
                        return getErrorResponse(message, 503);
                    }                    

                    response = await _taxBracketGetter.GetTaxData(year);
                }

                var incomeTax = _taxCalculator.CalculateIncomeTax(income, response.Content);
                return getSuccessResponse(incomeTax);
            }
            catch(Exception e)
            {
                _logger.LogCritical($"Unexpected exception: {e.Message}");
                return getErrorResponse($"Unexpected error. Please check your inputs and try again.", 500);
            }
        }

        /// <summary>
        /// Takes the standard errors returned from the TaxBracket API and returns an error message string.
        /// </summary>
        /// <param name="apiException"></param>
        /// <returns></returns>
        private async Task<string> getFirstErrorMessage(ApiException apiException)
        {
            var errors = await apiException.GetContentAsAsync<Dictionary<string, List<Dictionary<string, string>>>>();
            return errors["errors"][0]["message"];
        }


        private IncomeTaxResponse getSuccessResponse(double incomeTax)
        {
            Response.StatusCode = 200;
            return new IncomeTaxResponse()
            {
                TotalTax = incomeTax,
                Message = "Success"
            };
        }

        private IncomeTaxResponse getErrorResponse(string message, int errorCode)
        {
            Response.StatusCode = errorCode;
            return new IncomeTaxResponse()
            {
                TotalTax = null,
                Message = message
            };
        }
    }
}
