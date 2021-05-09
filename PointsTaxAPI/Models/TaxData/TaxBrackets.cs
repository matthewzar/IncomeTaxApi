using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PointsTaxAPI.Models.TaxData
{
    /// <summary>
    /// A collection of tax brackets. Little more than a wrapper around a list for now.
    /// </summary>
    public class TaxBracketCollection
    {
        [JsonPropertyName("tax_brackets")]
        public List<TaxBracket> Brackets { get; set; }

        // TODO - consider adding bracket-verification method to make sure that all the brackets are valid.
        // eg no mix/max mixups, no missing boundaries, etc.
    }
}
