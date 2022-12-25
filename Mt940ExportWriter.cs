using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Exthand.FinanceExports
{


    /// <summary>
    /// Class for writing to comma-separated-value (CSV) Transactions history files.
    /// </summary>
    public class Mt940ExportWriter : IExportWriter, IDisposable
    {
        // Private members
        private readonly StreamWriter _Writer;
        private readonly Settings _Mt940Settings;

        /// <summary>
        /// Initializes a new <see cref="CsvWriter"/> instance for the specified file
        /// using the specified character encoding.
        /// </summary>
        /// <param name="path">The name of the CSV file to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public Mt940ExportWriter(Stream stream, Encoding encoding, Settings settings)
        {
            _Writer = new(stream, encoding);
            _Mt940Settings = settings;
        }

        private string Clean(string text, int length = 9999)
        {
            if (length==9999 || (text.Length<length))
                return text.Replace("\n", " - ");
            else
                return text.Replace("\n", " - ").Substring(0, length);
        }

        private string GetMoney(decimal amount)
        {
            if (amount >= 0)
                return "C" + Math.Abs(amount).ToString("0.00");
            return "D" + Math.Abs(amount).ToString("0.00"); 
        }


        /// <summary>
        /// Asynchronously writes all Transaction to the stream.
        /// </summary>
        /// <param name="columns">The list of columns to write.</param>
        public async Task WriteAsync(TransactionList transactionList)
        {

            await _Writer.WriteLineAsync(":20:" + transactionList.transactionId);
            await _Writer.WriteLineAsync(":25:" + transactionList.IBANAccount + transactionList.Currency);
            await _Writer.WriteLineAsync(":28C:00001");
            if (transactionList.BalanceOpening>=0)
            { await _Writer.WriteLineAsync($":60F:C{transactionList.DateFirsTransaction.ToString("yyMMdd")}{transactionList.Currency}{Math.Abs(transactionList.BalanceOpening).ToString("0.00")}"); }
            else
            { await _Writer.WriteLineAsync($":60F:D{transactionList.DateFirsTransaction.ToString("yyMMdd")}{transactionList.Currency}{Math.Abs(transactionList.BalanceOpening).ToString("0.00")}"); } 
            

            foreach (Transaction transaction in transactionList.transactions)
            {
                await _Writer.WriteAsync(":61:" + transaction.DateValue.Value.ToString("yyMMdd") + transaction.DateExecution.Value.ToString("MMdd") + GetMoney(transaction.Amount));
                await _Writer.WriteLineAsync("N099//" + Clean(transaction.RemittanceUnstructured,16));
                await _Writer.WriteLineAsync(":86:");
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

