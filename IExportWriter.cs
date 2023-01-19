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
    public interface IExportWriter
    {

        /// <summary>
        /// Asynchronously writes all Transaction to the stream.
        /// </summary>
        /// <param name="columns">The list of columns to write.</param>
        public Task WriteAsync(TransactionList transactionList);


        /// <summary>
        /// Clears all buffers and causes any unbuffered data to be written to the underlying stream.
        /// </summary>
        public void Flush();

        /// <summary>
        /// Closes the file and the underlying stream.
        /// </summary>
        public void Close();

        /// <summary>
        /// Releases resources used by the object. 
        /// </summary>
        public void Dispose();
    }
}

