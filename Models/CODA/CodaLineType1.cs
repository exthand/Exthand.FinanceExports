using Exthand.FinanceExports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 1. "Old Balance"
    /// 1ABBBCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCDDDDDDDDDDDDDDDDEEEEEEFFFFFFFFFFFFFFFFFFFFFFFFFFGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGHHH
    /// 
    /// Examples:
    /// 12024BE91732047678076                  EUR1000000006025710080320EXTHAND SPRL              Compte Business One CBC            001
    /// 
    /// A -  1  N - Account structure
    ///             0 = Belgian account number
    ///             1 = foreign account number
    ///             2 = IBAN of the Belgian account number
    ///             3 = IBAN of the foreign account number
    /// B -  3  N - Sequence number statement
    /// C - 37 AN - Account number and currency code
    /// D - 16  N - Old balance (sign + amount)
    /// E -  6  N - Old balance date
    /// F - 26 AN - Name of the account holder
    /// G - 35 AN - Account description
    /// H -  3  N - Sequence number of the coded statement of account or zeros
    /// </summary>
    public class CodaLineType1 : ICodaLineType
    {
        public int StatementSequenceNumber { get; set; }
        public Account Account { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountDescription { get; set; }
        public decimal Balance { get; set; }
        public DateTime BalanceDate { get; set; }
        public int SequenceNumber { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.OldBalance;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append('1');
            stringBuilder.Append((int)Account.AccountType);
            stringBuilder.Append($"{StatementSequenceNumber:000}");
            stringBuilder.Append(Account.AccountNumberAndCurrencyToString());
            stringBuilder.Append(CodaStringHelper.FormatBalance(Balance, 15));
            stringBuilder.Append($"{BalanceDate:ddMMyy}");
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(AccountHolderName, 26));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(AccountDescription, 35));
            stringBuilder.Append($"{SequenceNumber:000}");
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Validate the Line
        /// </summary>
        public void Validate()
        {
            var errors = new List<string>();

            if ((StatementSequenceNumber < 0) || (StatementSequenceNumber > 999))
                errors.Add("0 <= StatementSequenceNumber <= 999");
            if (Account == null)
                errors.Add("Account is null");
            if (Account.IsCounterParty)
                errors.Add("Account is flagged as Counterparty");
            if ((SequenceNumber < 0) || (SequenceNumber > 999))
                errors.Add("0 <= SequenceNumber <= 999");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}