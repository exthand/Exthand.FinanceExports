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

        /// <summary>
        /// Initializes a new <see cref="CsvBuilder"/> instance for the specified file
        /// </summary>
        public CsvBuilder()
        {
            CsvLines = new List<string>();
            ResetBuilder();
        }


        /// <summary>
        /// Writes a single header line on top of the file.
        /// </summary>
        /// <param name="transaction">One Transaction instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        private string WriteHeaders()
        {

            StringBuilder stringBuilder = new();

            stringBuilder.Append($"ID{Delimiter}");
            stringBuilder.Append($"Amount{Delimiter}");
            stringBuilder.Append($"Currency{Delimiter}");
            stringBuilder.Append($"Name{Delimiter}");
            stringBuilder.Append($"BankAccount{Delimiter}");
            stringBuilder.Append($"CounterpartName{Delimiter}");
            stringBuilder.Append($"CounterpartIBAN{Delimiter}");
            stringBuilder.Append($"CounterpartReference{Delimiter}");
            stringBuilder.Append($"RemittanceUnstructured{Delimiter}");
            stringBuilder.Append($"RemoteId{Delimiter}");
            stringBuilder.Append($"End2EndId{Delimiter}");

            stringBuilder.Append($"DateExecution{Delimiter}");
            stringBuilder.Append($"DateValue{Delimiter}");

            stringBuilder.Append($"BalanceBefore{Delimiter}");
            stringBuilder.Append($"BalanceAfter" );

            return stringBuilder.ToString();
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
            stringBuilder.Append($"{transaction.BalanceAfter.ToString()}");

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
            CsvLines.Add(WriteHeaders());

            foreach (Transaction transaction in transactionList.Transactions)
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
        /// Get the content of the CSV file as one string
        /// </summary>
        public override string GetResultAsString()
        {
            return string.Join(Environment.NewLine, CsvLines.Select(cl => cl.ToString()));
        }

        /// <summary>
        /// Get the content of the CSV file as a collection of strings
        /// </summary>
        public override IEnumerable<string> GetResultAsLines()
        {
            return CsvLines.Select(cl => cl.ToString());
        }

        /// <summary>
        /// Get the content of the CSV file in a stream
        /// </summary>
        public override Stream GetResultAsStream()
        {
            var memoryStream = new MemoryStream();

            foreach (var csvLine in CsvLines)
            {
                memoryStream.Write(Encoding.UTF8.GetBytes(csvLine.ToString() + Environment.NewLine));
            }

            return memoryStream;
        }


    }
}

