using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exthand.FinanceExports.Models.Coda;

namespace Exthand.FinanceExports.Builders
{


    /// <summary>
    /// Class for writing to comma-separated-value (CSV) Transactions history files.
    /// </summary>
    public class CsvBuilder : BaseBuilder
    {
        public IList<string> CsvLines { get; private set; }
        private string Delimiter { get; } = ";";
        private string NewLine { get; } = "\n";

        /// <summary>
        /// Initializes a new <see cref="CsvBuilder"/> instance for the specified file
        /// </summary>
        public CsvBuilder()
        {
            CsvLines = new List<string>();
            ResetBuilder();
        }

        /// <summary>
        /// Writes a single line of Transaction to the list of lines.
        /// </summary>
        /// <param name="transaction">One Transaction instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        private string WriteTransaction(Transaction transaction)
        {
            // Verify required argument
            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            StringBuilder stringBuilder = new();

            stringBuilder.Append($"{transaction.Id}{Delimiter}");
            stringBuilder.Append($"{transaction.Amount.ToString() + Delimiter}");
            stringBuilder.Append($"{transaction.Currency + Delimiter}");
            stringBuilder.Append($"{transaction.IBANName + Delimiter}");
            stringBuilder.Append($"{transaction.IBANAccount + Delimiter}");
            stringBuilder.Append($"{transaction.CounterpartName + Delimiter}");
            stringBuilder.Append($"{transaction.CounterpartIBAN + Delimiter}");
            stringBuilder.Append($"{transaction.CounterpartReference + Delimiter}");
            stringBuilder.Append($"{transaction.RemittanceUnstructured + Delimiter}");
            stringBuilder.Append($"{transaction.RemoteId + Delimiter}");
            stringBuilder.Append($"{transaction.End2EndId + Delimiter}");

            stringBuilder.Append($"{transaction.DateExecution?.ToString("yyyyMMdd") + Delimiter}");
            stringBuilder.Append($"{transaction.DateValue?.ToString("yyyyMMdd") + Delimiter}");

            stringBuilder.Append($"{transaction.BalanceBefore.ToString() + Delimiter}");
            stringBuilder.Append($"{transaction.BalanceAfter.ToString()}{Delimiter}{NewLine}");

            return stringBuilder.ToString();
        }


        private string Clean(string text)
        {
            if (!string.IsNullOrEmpty(text))
                return text.Replace("\n", " | ").Replace(Delimiter, " ");
            else
                return "";
        }

        /// <summary>
        /// Reset all internal properties in case the builder is reused
        /// </summary>
        private void ResetBuilder()
        {
            CsvLines.Clear();
        }

        /// <summary>
        /// Asynchronously writes all Transaction to the stream.
        /// </summary>
        /// <param name="columns">The list of columns to write.</param>
        public override void Build(TransactionList transactionList)
        {
            //TODO: complete this one : WriteHeaders();

            foreach (Transaction transaction in transactionList.transactions)
            {
                transaction.Bank = Clean(transaction.Bank);
                transaction.CounterpartName = Clean(transaction.CounterpartName);
                transaction.CounterpartReference = Clean(transaction.CounterpartReference);
                transaction.IBANName = Clean(transaction.IBANName);
                transaction.RemittanceUnstructured = Clean(transaction.RemittanceUnstructured);
                CsvLines.Add(WriteTransaction(transaction));
            }
            return;
        }

        /// <summary>
        /// Get the content of the MT940 file as one string
        /// </summary>
        public override string GetResultAsString()
        {
            return string.Join('\n', CsvLines.Select(cl => cl.ToString()));
        }

        /// <summary>
        /// Get the content of the MT940 file as a collection of strings
        /// </summary>
        public override IEnumerable<string> GetResultAsLines()
        {
            return CsvLines.Select(cl => cl.ToString());
        }

        /// <summary>
        /// Get the content of the MT940 file in a stream
        /// </summary>
        public override Stream GetResultAsStream()
        {
            var memoryStream = new MemoryStream();

            foreach (var mt940Line in CsvLines)
            {
                memoryStream.Write(Encoding.UTF8.GetBytes(mt940Line.ToString() + "\n"));
            }

            return memoryStream;
        }

    }
}

