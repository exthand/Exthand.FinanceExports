using System.Collections.Generic;
using System.IO;

namespace Exthand.FinanceExports
{
    public interface IBaseExportBuilder<T>
    {
        /// <summary>
        /// Build the content of the file
        /// </summary>
        void Build(T request);

        string GetResultAsString();

        /// <summary>
        /// Get the content of the file as a collection of strings
        /// </summary>
        IEnumerable<string> GetResultAsLines();

        /// <summary>
        /// Get the content of the file in a stream
        /// </summary>
        Stream GetResultAsStream();

        /// <summary>
        /// Get the content of the file in a byte array
        /// </summary>
        byte[] GetResultAsBytes();
    }
}
