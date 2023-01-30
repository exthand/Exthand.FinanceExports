using Exthand.FinanceExports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 3.3. "Information Record"
    /// 33AAAABBBBCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC                         0 D
    /// 
    /// Examples:
    /// 3300010001SOME INFORMATION ABOUT THIS TRANSACTION                                                                            0 0
    /// 
    /// A -  4  N - Continuous sequence number
    /// B -  4  N - Detail number
    /// C - 90 AN - Communication
    /// D -  1  N - Link code with next data record
    ///               0 = no information record is following (data record 3)
    ///               1 = an information record is following
    /// </summary>
    public class CodaLineType33 : ICodaLineType
    {
        public int ContinuousSequenceNumber { get; set; }
        public int DetailNumber { get; set; }
        public string Communication { get; set; }
        public bool LinkCode { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.InformationPart3;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("33");
            stringBuilder.Append($"{ContinuousSequenceNumber:0000}");
            stringBuilder.Append($"{DetailNumber:0000}");
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(Communication, 90));
            stringBuilder.Append(new string(' ', 25));
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
                errors.Add("0 <= SequenceNumber <= 999");
            if ((DetailNumber < 0) || (DetailNumber > 999))
                errors.Add("0 <= DetailNumber <= 999");
            if (string.IsNullOrEmpty(Communication))
                errors.Add("Communication is null or empty");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}
