using Exthand.FinanceExports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Models.Coda
{
    /// <summary>
    /// Coda Line Type 2.1. "Movement Record"
    /// 21AAAABBBBCCCCCCCCCCCCCCCCCCCCCDEEEEEEEEEEEEEEEFFFFFFGGGGGGGGHIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIJJJJJJKKKLM N
    /// 
    /// Examples:
    /// 2100010000OL2ES7AMF SIPOVSOVERS1000000001210000090320001010000Fact 2020006                                         09032002401 0
    /// 2100010000JRFC00120DSCCOCACAERT0000000000005000111017001500001101000003505158                                      11101713901 0
    /// 21000100003403076534383000143  1000000000009680300617335370000Zichtrekening nr  21354598                           03071707411 0
    /// 2100010000PSUF00968 TK4TBNINNIG1000000000812690301017313010000Message goes here                                    30101710311 0
    /// 2100040000310N019418263001137  0000000017233540310517001500000Europese overschrijving (zie bijlage)                31051710101 0
    /// 21000100000001200002835        0000000001767820251214001120000112/4554/46812   813                                 25121421401 0
    /// 21000100000001200002835        1000000000767823251214001120000112/4554/46812   813                                 25121421401 0
    /// 
    /// A -  4  N - Continuous sequence number (Starts at 0001 and is increased by 1 until 0000 then 0001 again)
    /// B -  4  N - Detail number (Starts at 0001 and is increased by 1 until 0000 then 0001 again)
    /// C - 21 AN - Reference number of the bank
    /// D -  1  N - Movement sign
    ///               0 = credit
    ///               1 = debit
    /// E - 15  N - Amount (12 pos + 3 decimals)
    /// F -  6  N - Date ddMMyy - 000000 if unknown
    /// G -  8  N - Transaction code
    /// H -  1  N - Communication type: 
    ///               0 = unstructured
    ///               1 = structured
    /// I - 53 AN - Communication zone
    /// J -  6  N - Entry Date ddMMyy
    /// K -  3  N - Sequence number statement
    /// L -  1  N - Globalisation code
    /// M -  1  N - Next code: 
    ///               0 = no record 2 or 3 with record identification 2 is following
    ///               1 = a record 2 or 3 with record identification 2 is following
    /// N -  1  N - Link code with next data record
    ///               0 = no information record is following (data record 3)
    ///               1 = an information record is following
    /// </summary>
    public class CodaLineType21 : ICodaLineType
    {
        public int ContinuousSequenceNumber { get; set; }
        public int DetailNumber { get; set; }
        public string BankReferenceNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ValueDate { get; set; }
        public int TransactionCode { get; set; }
        public CommunicationType CommunicationType { get; set; }
        public string Communication { get; set; }
        public DateTime? EntryDate { get; set; }
        public int SequenceNumberStatement { get; set; }
        public int GlobalisationCode { get; set; }
        public bool NextCode { get; set; }
        public bool LinkCode { get; set; }


        /// <summary>
        /// Get Coda Line Type
        /// </summary>
        public CodaLineType GetCodaLineType() => CodaLineType.TransactionPart1;


        /// <summary>
        /// Print the line in CODA format
        /// </summary>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("21");
            stringBuilder.Append($"{ContinuousSequenceNumber:0000}");
            stringBuilder.Append($"{DetailNumber:0000}");
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(BankReferenceNumber, 21));
            stringBuilder.Append(CodaStringHelper.FormatBalance(Amount, 15));
            stringBuilder.Append(ValueDate.HasValue ? $"{ValueDate:ddMMyy}" : "000000");
            stringBuilder.Append($"{TransactionCode:00000000}");
            stringBuilder.Append((int)CommunicationType);
            stringBuilder.Append(CodaStringHelper.TrucateOrPadRight(Communication, 53));
            stringBuilder.Append($"{EntryDate.Value:ddMMyy}");
            stringBuilder.Append($"{SequenceNumberStatement:000}");
            stringBuilder.Append($"{GlobalisationCode:0}");
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

            if ((ContinuousSequenceNumber < 0) || (ContinuousSequenceNumber > 9999))
                errors.Add("0 <= ContinuousSequenceNumber <= 9999");
            if ((DetailNumber < 0) || (DetailNumber > 9999))
                errors.Add("0 <= DetailNumber <= 9999");
            if (string.IsNullOrEmpty(BankReferenceNumber))
                errors.Add("Bank reference number is null or empty");
            if (Amount == 0)
                errors.Add("Amount is 0");
            if ((TransactionCode < 0) || (TransactionCode > 99999999))
                errors.Add("0 <= TransactionCode <= 99999999");
            if (Communication == null)
                errors.Add("Communication is null");
            if (EntryDate == null)
                errors.Add("EntryDate is null");
            if ((SequenceNumberStatement < 0) || (SequenceNumberStatement > 999))
                errors.Add("0 <= SequenceNumberStatement <= 999");
            if ((GlobalisationCode < 0) || (GlobalisationCode > 9))
                errors.Add("0 <= GlobalisationCode <= 9");

            if (errors.Any())
                throw new CodaValidationException($"Validation error {GetType().Name}", errors);
        }
    }
}