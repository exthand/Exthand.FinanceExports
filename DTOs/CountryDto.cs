using System;
using System.Collections.Generic;
using System.Text;

namespace Exthand.FinanceExports.DTOs
{
    public class CountryDto
    {
        //[SwaggerSchema("The 01Financials internal id of the country (see [ISO](https://www.iso.org/iso-3166-country-codes.html))")]
        public string Iso { get; set; }

        //[SwaggerSchema("The human readable English country name")]
        public string NiceName { get; set; }
    }
}