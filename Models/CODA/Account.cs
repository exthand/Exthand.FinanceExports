using Exthand.FinanceExports.Helpers;

namespace Exthand.FinanceExports.Models.Coda
{
    public class Account
    {
        public string AccountNumber { get; set; }
        public CodaAccountType AccountType { get; set; }
        public string CurrencyCode { get; set; }
        public string ExtensionZone { get; set; }
        public bool IsCounterParty { get; set; }
        public int? QualitificationCode { get; set; }

        /// <summary>
        /// 37 AN - Account number and currency code
        /// </summary>
        public string AccountNumberAndCurrencyToString()
        {
            return AccountType switch
            {
                CodaAccountType.Belgian => BelgianAccountToString(),
                CodaAccountType.BelgianIban => BelgianIbanAccountToString(),
                CodaAccountType.Foreign => ForeignAccountToString(),
                CodaAccountType.ForeignIban => ForeignIbanAccountToString(),
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Account Type: Belgian (0)
        /// - 12  N - Belgian account number 
        /// -  1 AN - blank
        /// -  3 AN - ISO currency code or blank
        /// -  1  N - qualification code or blank
        /// -  2 AN - ISO country code or blank
        /// -  3 AN - blank spaces
        /// - 15 AN - extension zone or blank
        /// </summary>
        private string BelgianAccountToString()
        {
            var accountNumber = CodaStringHelper.TrucateOrPadRight(AccountNumber, 12);
            var currencyCode = CodaStringHelper.TrucateOrPadRight(CurrencyCode, 3);
            var qualificationCode = QualitificationCode.HasValue ? $"{QualitificationCode:0}" : " ";
            var countryCode = GetCountryCode(AccountNumber) ?? "  ";

            var extensionZone = CodaStringHelper.TrucateOrPadRight(ExtensionZone, 15);

            return $"{accountNumber} {currencyCode}{qualificationCode}{countryCode}   {extensionZone}";
        }


        /// <summary>
        /// Account Type: BelgianIban (2)
        /// - 31 AN - IBAN (Belgian number)
        /// -  3 AN - extension zone or blank
        /// -  3 AN - ISO currency code of the account (optional for counterpary)
        /// </summary>
        private string BelgianIbanAccountToString()
        {
            var iban = CodaStringHelper.TrucateOrPadRight(AccountNumber, 31);
            var extensionZone = CodaStringHelper.TrucateOrPadRight(ExtensionZone, 3);
            var currencyCode = CodaStringHelper.TrucateOrPadRight(CurrencyCode, 3);

            return $"{iban}{extensionZone}{currencyCode}";
        }


        /// <summary>
        /// Account Type: Foreign (1)
        /// - 34 AN - foreign account number
        /// -  3 AN - ISO currency code of the account (optional for counterpary)
        /// </summary>
        private string ForeignAccountToString()
        {
            var accountNumber = CodaStringHelper.TrucateOrPadRight(AccountNumber, 34);
            var currencyCode = CodaStringHelper.TrucateOrPadRight(CurrencyCode, 3);

            return $"{accountNumber}{currencyCode}";
        }


        /// <summary>
        /// Account Type: ForeignIban (3)
        /// - 34 AN - IBAN (foreign account number)
        /// -  3 AN - ISO currency code of the account (optional for counterpary)
        /// </summary>
        private string ForeignIbanAccountToString()
        {
            var iban = CodaStringHelper.TrucateOrPadRight(AccountNumber, 34);
            var currencyCode = CodaStringHelper.TrucateOrPadRight(CurrencyCode, 3);

            return $"{iban}{currencyCode}";
        }


        /// <summary>
        /// Get Country Code from account number
        /// </summary>
        private string GetCountryCode(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber) && accountNumber.Length > 2 && char.IsLetter(accountNumber[0]) && char.IsLetter(accountNumber[1]))
                return accountNumber.Substring(0, 2).ToUpper();

            return null;
        }
    }
}
