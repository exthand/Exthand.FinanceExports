using Exthand.FinanceExports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 2.2. "Movement Record"
    /// 22AAAABBBBCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDEEEEEEEEEEE   FGGGGHHHHIIIIJ K
    /// 
    /// Examples:
    /// 2200010000                                                                                        CREGBEBBXXX                1 0
    /// 2200040000 + 17.233,54Van: COMPANY BLABLABLAH BVBA - BE64NOT PROVIDED BBRUBEBB                                               1 0
    /// 
    /// A -  4  N - Continuous sequence number
    /// B -  4  N - Detail number
    /// C - 53 AN - Communication
    /// D - 35 AN - Customer reference or blank
    /// E - 11 AN - BIC (8 or 11 characters) of the counterparty's bank or blank
    /// F -  1  N - Type of R-transaction or blank 
    ///               1 : Reject
    ///               2 : Return
    ///               3 : Refund
    ///               4 : Reversal
    ///               5 : Cancellation
    /// G -  4 AN - ISO Reason Return Code or blanks
    /// H -  4 AN - Category Purpose
    /// I -  4 AN - Purpose
    /// J -  1  N - Next code: 
    ///               0 = no record 3 with record identification 2 is following
    ///               1 = a record 3 with record identification 2 is following
    /// K -  1  N - Link code with next data record
    ///               0 = no information record is following (data record 3)
    ///               1 = an information record is following
    /// </summary>
    public class CodaLineType22 : ICodaLineType
    {
        public int ContinuousSequenceNumber { get; set; }
        public int DetailNumber { get; set; }
        public string Communication { get; set; }
        public string CustomerReference { get; set; }
        public string CounterpartyBic { get; set; }
        public TransactionType TransactionType { get; set; }
        public string ISOReasonReturnCode { get; set; }
        public string PurposeCategory { get; set; }
        public string Purpose { get; set; }
        public bool NextCode { get; set; }
        public bool LinkCode { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.TransactionPart2;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("22");
            stringBuilder.Append($"{ContinuousSequenceNumber:0000}");
            stringBuilder.Append($"{DetailNumber:0000}");
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(CodaStringHelper.Substring(Communication, 53, 53), 53));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(CustomerReference, 35));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(CounterpartyBic, 11));
            stringBuilder.Append(new string(' ', 3));
            stringBuilder.Append(TransactionType == TransactionType.Unknown ? " " : $"{(int)TransactionType}");
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(ISOReasonReturnCode, 4));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(PurposeCategory, 4));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(Purpose, 4));
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

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}