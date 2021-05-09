using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PointsTaxAPI.Models.TaxData
{
    /// <summary>
    /// A single tax bracket, with a min and max bound, plus the tax rate for income inside said bounds.
    /// </summary>
    public class TaxBracket
    {
        [JsonPropertyName("max")]
        public uint Max { get; set; } = uint.MaxValue;

        [JsonPropertyName("min")]
        public uint Min { get; set; } = uint.MinValue;

        [JsonPropertyName("rate")]
        public dynamic Rate
        {
            get { return rate; }
            set
            {
                // TODO: see if there's a better way to deserialise a property that can have multiple mixed types.
                // This works, but it feels like a kludge.
                if (value.GetType() == typeof(string))
                {
                    rate = double.Parse(value.ToString());
                }
                else rate = (double)value;
            }
        }
        private double rate;

        // TODO - migrate the Refit api wrapper so it uses this factory during request deserialisation
        public static TaxBracket BuildTaxBracket(uint min, uint max, double rate, out bool valid)
        {
            valid = max > min && 
                    rate >= 0 && 
                    rate <= 1;

            return new TaxBracket()
            {
                Max = max,
                Min = min,
                Rate = rate
            };
        }
    }
}
