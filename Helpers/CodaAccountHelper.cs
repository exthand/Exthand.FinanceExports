﻿using Exthand.FinanceExports.Models.Coda;
using System;

namespace Exthand.FinanceExports.Helpers
{
    public static class CodaAccountHelper
    {

        public static Account ParseAccount(string iban, string currency)
        {
            if (!string.IsNullOrEmpty(iban) && iban.Length >= 15 && char.IsLetter(iban[0]) && char.IsLetter(iban[1]))
            {
                return new Account
                {
                    AccountType = iban.StartsWith("BE") ? CodaAccountType.BelgianIban : CodaAccountType.ForeignIban,
                    AccountNumber = iban,
                    CurrencyCode = currency
                };
            }

            return new Account
            {
                AccountType = CodaAccountType.Unknown,
                AccountNumber = "",
                CurrencyCode = currency ?? "EUR"
            };
        }
    }
}
