using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Exthand.FinanceExports
{

    public class Settings
    {
        public string Delimiter { get { return ";"; } }
    }

    public enum ExportType : int
    {
        CSV = 100,
        MT940 = 200,
        CAMT053 = 300,
        CODA = 900
    }


    /// <summary>
    /// Class for writing to comma-separated-value (CSV) Transactions history files.
    /// </summary>
    public class Writer : IDisposable
    {
        // Private members
        private readonly Settings _settings;
        private readonly ExportType _exportType;
        private readonly IExportWriter _exportWriter;
        private readonly MemoryStream _memoryStream;

        /// <summary>
        /// Initializes a new <see cref="CsvWriter"/> instance for the specified file
        /// using the specified character encoding.
        /// </summary>
        /// <param name="path">The name of the CSV file to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public Writer(Encoding encoding, ExportType exportType )
        {
            _memoryStream = new MemoryStream();
            _exportType = exportType;
            _settings = new();
            switch(exportType)
            {
                case ExportType.CSV:
                    _exportWriter = new CsvExportWriter(_memoryStream, encoding, _settings);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Asynchronously writes all Transaction to the stream.
        /// </summary>
        /// <param name="columns">The list of columns to write.</param>
        public async Task WriteAsync(IEnumerable<Transaction> transactions)
        {
            await _exportWriter.WriteAsync(transactions);
            return;
        }

        public byte[] GetBuffer() => _memoryStream.GetBuffer();


        /// <summary>
        /// Clears all buffers and causes any unbuffered data to be written to the underlying stream.
        /// </summary>
        public void Flush() => _exportWriter.Flush();


        /// <summary>
        /// Closes the file and the underlying stream.
        /// </summary>
        public void Close() => _exportWriter.Close();

        /// <summary>
        /// Releases resources used by the object. 
        /// </summary>
        public void Dispose()
        {
            _exportWriter.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

