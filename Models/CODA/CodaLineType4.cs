using Exthand.FinanceExports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 4. "Free communication"
    /// 4 AAAABBBB                      CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC               D
    /// 
    /// Examples:
    /// 4 00010005                      THIS IS A PUBLIC MESSAGE                                                                       0
    /// 
    /// A -  4  N - Continuous sequence number
    /// B -  4  N - Detail number
    /// C - 80 AN - Communication
    /// D -  1  N - Link code with next data record
    ///               0 = no free communication is following
    ///               1 = a free communication is following
    /// </summary>
    public class CodaLineType4 : ICodaLineType
    {
        public int SequenceNumber { get; set; }
        public int DetailNumber { get; set; }
        public string Communication { get; set; }
        public bool LinkCode { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.Message;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("4 ");
            stringBuilder.Append($"{SequenceNumber:0000}");
            stringBuilder.Append($"{DetailNumber:0000}");
            stringBuilder.Append(new string(' ', 22));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(Communication, 80));
            stringBuilder.Append(new string(' ', 15));
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
            if ((DetailNumber < 0) || (DetailNumber > 999))
                errors.Add("0 <= DetailNumber <= 999");
            if (string.IsNullOrEmpty(Communication))
                errors.Add("Communication is null or empty");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}
