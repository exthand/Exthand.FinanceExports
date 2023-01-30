using Exthand.FinanceExports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 0. "Header Record"
    /// 00000AAAAAABBB05C       DDDDDDDDDDEEEEEEEEEEEEEEEEEEEEEEEEEEFFFFFFFFFFFGGGGGGGGGGG HHHHHIIIIIIIIIIIIIIIIJJJJJJJJJJJJJJJJ       2
    /// 
    /// Examples:
    /// 0000009032019005        00480212  DEBAILLE JEAN-GABRIEL     CREGBEBB   00701961581 00000                                       2
    /// 
    /// A -  6  N - Creation date (DDMMYY)
    /// B -  3  N - Bank identification number or zeros
    /// C -  1 AN - If duplicate "D", otherwise blank
    /// D - 10 AN - file reference as determined by the bank or blank
    /// E - 26 AN - Name addressee (bank account holder get from bank)
    /// F - 11 AN - BIC of the bank holding the account (8 characters followed by 3 blanks or 11 characters)
    /// G - 11  N - Identification number of the Belgium-based account holder: 0 + company number
    /// H -  5  N - Code "separate application"
    /// I - 16 AN - Blank or Transaction reference
    /// J - 16 AN - Blank or Related reference
    /// </summary>
    public class CodaLineType0 : ICodaLineType
    {
        public DateTime? CreationDate { get; set; }
        public int BankIdentificationNumber { get; set; }
        public bool Duplicate { get; set; }
        public string FileReference { get; set; }
        public string AccountHolderName { get; set; }
        public string Bic { get; set; }
        public int CompanyNumber { get; set; }
        public int SeparateApplication { get; set; }
        public string TransactionReference { get; set; }
        public string RelatedReference { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.Header;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("00000");
            stringBuilder.Append($"{CreationDate.Value:ddMMyy}");
            stringBuilder.Append($"{BankIdentificationNumber:000}");
            stringBuilder.Append("05");
            stringBuilder.Append(Duplicate ? "D" : " ");
            stringBuilder.Append(new string(' ', 7));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(FileReference, 10));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(AccountHolderName, 26));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(Bic, 11));
            stringBuilder.Append($"{CompanyNumber:00000000000}");
            stringBuilder.Append(" ");
            stringBuilder.Append($"{SeparateApplication:00000}");
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(TransactionReference, 16));
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(RelatedReference, 16));
            stringBuilder.Append(new string(' ', 7));
            stringBuilder.Append("2");
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Validate the Line
        /// </summary>
        public void Validate()
        {
            var errors = new List<string>();

            if(CreationDate == null)
                errors.Add("CreationDate is null");
            if ((BankIdentificationNumber < 0) || (BankIdentificationNumber > 999))
                errors.Add("0 <= BankIdentificationNumber <= 999");
            if (FileReference == null)
                errors.Add("FileReference is null");
            if (string.IsNullOrEmpty(AccountHolderName))
                errors.Add("AccountHolderName is null or empty");
            if (string.IsNullOrEmpty(Bic))
                errors.Add("Bic is null or empty");
            // TODO: validate company number
            if ((SeparateApplication < 0) || (SeparateApplication > 99999))
                errors.Add("0 <= SeparateApplication <= 99999");
            if (TransactionReference == null)
                errors.Add("TransactionReference is null");
            if (RelatedReference == null)
                errors.Add("RelatedReference is null");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}