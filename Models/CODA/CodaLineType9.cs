using Exthand.FinanceExports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 9. "Trailer Record"
    /// 9               AAAAAABBBBBBBBBBBBBBBCCCCCCCCCCCCCCC                                                                           D
    /// 
    /// Examples:
    /// 9               000015000000003352220000000000700000                                                                           2
    /// 
    /// A -  6  N - Number of records 1, 2.1, 2.2, 2.3, 3.1, 3.2, 3.3 and 8
    /// B - 15  N - Debit movement
    ///               Sum of the amounts in type 2 records with detail number 0000
    /// C - 15 AN - Credit movement
    ///               Sum of the amounts in type 2 records with detail number 0000
    /// D -  1  N - Multiple file code
    ///               1 = another file is following
    ///               2 = last file
    /// </summary>
    public class CodaLineType9 : ICodaLineType
    {
        public int NumberOfRecors { get; set; }
        public decimal DebitMovement { get; set; }
        public decimal CreditMovement { get; set; }
        public bool MultipleFiles { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.Footer;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("9");
            stringBuilder.Append(new string(' ', 15));
            stringBuilder.Append($"{NumberOfRecors:000000}");
            stringBuilder.Append(CodaStringHelper.FormatBalance(DebitMovement, 15, includeSign: false));
            stringBuilder.Append(CodaStringHelper.FormatBalance(CreditMovement, 15, includeSign: false));
            stringBuilder.Append(new string(' ', 75));
            stringBuilder.Append(MultipleFiles ? "1" : "2");
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Validate the Line
        /// </summary>
        public void Validate()
        {
            var errors = new List<string>();

            if ((NumberOfRecors <= 0) || (NumberOfRecors > 999999))
                errors.Add("0 < NumberOfRecors <= 999999");
            if (DebitMovement < 0)
                errors.Add("DebitMovement < 0");
            if (CreditMovement < 0)
                errors.Add("CreditMovement < 0");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}
