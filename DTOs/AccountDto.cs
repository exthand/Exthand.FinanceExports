using System;
using System.Collections.Generic;
using System.Text;

namespace Exthand.FinanceExports.DTOs
{
    //[SwaggerSchema("Represent a bank account")]

    public class AccountDto
    {
        //[SwaggerSchema("The system id of the account")]
        public Guid Id { get; set; }

        //[SwaggerSchema("The ISO4217 currency code")]
        public string Currency { get; set; }

        //[SwaggerSchema("The IBAN of the account")]
        public string Iban { get; set; }

        //[SwaggerSchema("The bank's description of the account if any")]
        public string Description { get; set; }

        //[SwaggerSchema("The consent validity end date")]
        public DateTime ValidUntil { get; set; }

        //[SwaggerSchema("A id to see which accounts are linked together by the same consent.")]
        public string ConsentGrouping { get; set; }
        public FinancialInstitutionDto FinancialInstitution { get; set; }

        //[SwaggerSchema("The nickname of the account.")]
        public string NickName { get; set; }

        //[SwaggerSchema("A list of comma separated tags attached to the account")]
        public string Tags { get; set; }
    }
}
