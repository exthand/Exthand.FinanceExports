using Exthand.FinanceExports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 8. "New Balance"
    /// 8AAABBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBCCCCCCCCCCCCCCCCDDDDDD                                                                E
    /// 
    /// Examples:
    /// 8024BE91732047678076                  EUR1000000008677930090320                                                                0
    /// 
    /// A -  3  N - Sequence number statement of account
    /// B - 37 AN - Account number and currency code
    /// C - 16  N - Balance (sign + amount)
    /// D -  6  N - New balance date DDMMYY
    /// E -  1  N - Link code with next data record
    ///               0 = no free communication is following (data record 4)
    ///               1 = a free communication is following
    /// </summary>
    public class CodaLineType8 : ICodaLineType
    {
        public int SequenceNumber { get; set; }
        public Account Account { get; set; }
        public decimal Balance { get; set; }
        public DateTime BalanceDate { get; set; }
        public bool LinkCode { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.NewBalance;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("8");
            stringBuilder.Append($"{SequenceNumber:000}");
            stringBuilder.Append(Account.AccountNumberAndCurrencyToString());
            stringBuilder.Append(CodaStringHelper.FormatBalance(Balance, 15));
            stringBuilder.Append($"{BalanceDate:ddMMyy}");
            stringBuilder.Append(new string(' ', 64));
            stringBuilder.Append(CodaStringHelper.FormatBoolean(LinkCode));
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Validate the Line
        /// </summary>
        public void Validate()
        {
            var errors = new List<string>();

            if ((SequenceNumber < 0) || (SequenceNumber > 999))
                errors.Add("0 <= SequenceNumber <= 999");
            if (Account == null)
                errors.Add("Account is null");
            if (Account.IsCounterParty)
                errors.Add("Account is flagged as Counterparty");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}
