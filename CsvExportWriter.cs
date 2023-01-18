using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Exthand.FinanceExports
{


    /// <summary>
    /// Class for writing to comma-separated-value (CSV) Transactions history files.
    /// </summary>
    public class CsvExportWriter : IExportWriter, IDisposable
    {
        // Private members
        private readonly StreamWriter _Writer;
        private readonly Settings _CsvSettings;

        /// <summary>
        /// Initializes a new <see cref="CsvWriter"/> instance for the specified file
        /// using the specified character encoding.
        /// </summary>
        /// <param name="path">The name of the CSV file to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public CsvExportWriter(Stream stream, Encoding encoding, Settings settings)
        {
            _Writer = new(stream, encoding);
            _CsvSettings = settings;
        }

        /// <summary>
        /// Writes a single line of Transaction to the file.
        /// </summary>
        /// <param name="transaction">One Transaction instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task WriteTransactionAsync(Transaction transaction)
        {
            // Verify required argument
            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            await _Writer.WriteAsync(transaction.Id + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.Amount.ToString() + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.Currency + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.IBANName + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.IBANAccount + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.CounterpartName + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.CounterpartIBAN + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.CounterpartReference + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.RemittanceUnstructured + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.RemoteId + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.End2EndId + _CsvSettings.Delimiter);

            await _Writer.WriteAsync(transaction.DateExecution?.ToString("yyyyMMdd") + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.DateValue?.ToString("yyyyMMdd") + _CsvSettings.Delimiter);

            await _Writer.WriteAsync(transaction.BalanceBefore.ToString() + _CsvSettings.Delimiter);
            await _Writer.WriteAsync(transaction.BalanceAfter.ToString() + _CsvSettings.Delimiter);

            await _Writer.WriteLineAsync();
            return;
        }


        private string Clean(string text)
        {
            if (!string.IsNullOrEmpty(text))
                return text.Replace("\n", " | ").Replace(_CsvSettings.Delimiter, " ");
            else
                return "";
        }


        /// <summary>
        /// Asynchronously writes all Transaction to the stream.
        /// </summary>
        /// <param name="columns">The list of columns to write.</param>
        public async Task WriteAsync(List<TransactionList> transactionListing)
        {
            //TODO: complete this one : WriteHeaders();

            foreach (TransactionList transactionList in transactionListing)
            {
                foreach (Transaction transaction in transactionList.transactions)
                {
                    transaction.Bank = Clean(transaction.Bank);
                    transaction.CounterpartName = Clean(transaction.CounterpartName);
                    transaction.CounterpartReference = Clean(transaction.CounterpartReference);
                    transaction.IBANName = Clean(transaction.IBANName);
                    transaction.RemittanceUnstructured = Clean(transaction.RemittanceUnstructured);
                    await WriteTransactionAsync(transaction);
                }
            }
            return;
        }

        public async Task<StreamWriter> GetStreamAsync() => _Writer;


        /// <summary>
        /// Clears all buffers and causes any unbuffered data to be written to the underlying stream.
        /// </summary>
        public void Flush() => _Writer.Flush();

        /// <summary>
        /// Closes the file and the underlying stream.
        /// </summary>
        public void Close() => _Writer.Close();

        /// <summary>
        /// Releases resources used by the object. 
        /// </summary>
        public void Dispose()
        {
            _Writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

