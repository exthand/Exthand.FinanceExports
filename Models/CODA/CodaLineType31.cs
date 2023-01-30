using Exthand.FinanceExports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 3.1. "Information Record"
    /// 31AAAABBBBCCCCCCCCCCCCCCCCCCCCCDDDDDDDDEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF            G H
    /// 
    /// Examples:
    /// 3100010001OL2ES7AMF SIPOVSOVERS001010001001LE CIEL & LA TERRE SRL                                                            0 0
    /// 
    /// A -  4  N - Continuous sequence number
    /// B -  4  N - Detail number
    /// C - 21 AN - Reference number added by the bank:
    /// D -  8  N - Transaction code
    /// E -  1  N - Communication type: 
    ///               0 = unstructured
    ///               1 = structured
    /// F - 73 AN - Communication
    /// G -  1  N - Next code: 
    ///               0 = no record 2 with record identification 3 is following
    ///               1 = a record 2 with record identification 3 is following
    /// H -  1  N - Link code with next data record
    ///               0 = no information record is following (data record 3)
    ///               1 = an information record is following
    /// </summary>
    public class CodaLineType31 : ICodaLineType
    {
        public int ContinuousSequenceNumber { get; set; }
        public int DetailNumber { get; set; }
        public string BankReferenceNumber { get; set; }
        public int TransactionCode { get; set; }
        public CommunicationType CommunicationType { get; set; }
        public string Communication { get; set; }
        public bool NextCode { get; set; }
        public bool LinkCode { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.InformationPart1;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("31");
            stringBuilder.Append($"{ContinuousSequenceNumber:0000}");
            stringBuilder.Append($"{DetailNumber:0000}");
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(BankReferenceNumber, 21));
            stringBuilder.Append($"{TransactionCode:00000000}");
            stringBuilder.Append((int)CommunicationType);
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(Communication, 73));
            stringBuilder.Append(new string(' ', 12));
            stringBuilder.Append(CodaStringHelper.FormatBoolean(NextCode));
            stringBuilder.Append(" ");
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
            if (string.IsNullOrEmpty(BankReferenceNumber))
                errors.Add("BankReferenceNumber is null or empty");
            if ((TransactionCode < 0) || (TransactionCode > 99999999))
                errors.Add("0 <= TransactionCode <= 99999999");
            if (string.IsNullOrEmpty(Communication))
                errors.Add("Communication is null or empty");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}
