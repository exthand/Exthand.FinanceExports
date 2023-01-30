using Exthand.FinanceExports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 2.3. "Movement Record"
    /// 23AAAABBBBCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE0 F
    /// 
    /// Examples:
    /// 2300010000BE71732039083169                     LE CIEL & LA TERRE SRL                                                        0 1
    /// 
    /// A -  4  N - Continuous sequence number
    /// B -  4  N - Detail number
    /// C - 37 AN - Counterparty's account number and currency code or blank
    /// D - 35 AN - Counterparty's name
    /// E - 43 AN - Communication
    /// F -  1  N - Link code with next data record
    ///               0 = no information record is following (data record 3)
    ///               1 = an information record is following
    /// </summary>
    public class CodaLineType23 : ICodaLineType
    {
        public int ContinuousSequenceNumber { get; set; }
        public int DetailNumber { get; set; }        
        public Account CounterpartyAccount { get; set; }
        public string CounterpartyName { get; set; }
        public string Communication { get; set; }
        public bool LinkCode { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.TransactionPart3;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("23");
            stringBuilder.Append($"{ContinuousSequenceNumber:0000}");
            stringBuilder.Append($"{DetailNumber:0000}");
            stringBuilder.Append(CounterpartyAccount.AccountNumberAndCurrencyToString());
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(CounterpartyName, 35));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(CodaStringHelper.Substring(Communication, 53+53, 43), 43));
            stringBuilder.Append("0 ");
            stringBuilder.Append(CodaStringHelper.FormatBoolean(LinkCode));
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Validate the Line
        /// </summary>
        public void Validate()
        {
            var errors = new List<string>();

            if ((ContinuousSequenceNumber < 0) || (ContinuousSequenceNumber > 999))
                errors.Add("0 <= ContinuousSequenceNumber <= 999");
            if ((DetailNumber < 0) || (DetailNumber > 999))
                errors.Add("0 <= DetailNumber <= 999");
            if (CounterpartyAccount == null)
                errors.Add("CounterpartyAccount is null");
            if (!CounterpartyAccount.IsCounterParty)
                errors.Add("CounterpartyAccount is not flagged as Counterparty");
            if (string.IsNullOrEmpty(CounterpartyName))
                errors.Add("CounterpartyName is null or empty");
            if (string.IsNullOrEmpty(Communication))
                errors.Add("Communication is null or empty");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}