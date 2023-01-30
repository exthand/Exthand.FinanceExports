using System;
using System.Collections.Generic;
using System.Text;

namespace Exthand.FinanceExports.DTOs
{
    public class FinancialInstitutionDto
    {
        //[SwaggerSchema("The 01Financials internal id of the financial institution")]
        public Guid Id { get; set; }

        //[SwaggerSchema("The full name to the financial institution")]
        public string Fullname { get; set; }

        //[SwaggerSchema("The connector id that identify internally the connector to be used.\n\nYou may use it when you add an account request for the PSU to preselect the bank")]
        public int ConnectorId { get; set; }

        //[SwaggerSchema("The country of this financial institution")]
        public CountryDto Country { get; set; }
    }
}