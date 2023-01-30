using System;
using System.Collections.Generic;
using System.Text;

namespace Exthand.FinanceExports.DTOs
{
    public class BalanceDto
    {
        //[SwaggerSchema("The 01Financials internal id of the object")]
        public Guid Id { get; set; }

        //[SwaggerSchema("The date 01Financials fetched this balance")]
        public DateTime RequestedAt { get; set; }

        //[SwaggerSchema("The balance type")] public string BalanceType { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public DateTime? LastChangeDateTime { get; set; }
        //[SwaggerSchema("The currency")] public string Currency { get; set; }
        //[SwaggerSchema("The balance amount")] public decimal Amount { get; set; }

        //[SwaggerSchema("The IBAN of this balance")]
        public string Iban { get; set; }
    }
}