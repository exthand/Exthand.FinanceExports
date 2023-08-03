using Exthand.FinanceExports.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Exthand.FinanceExports.Builders
{
    public abstract class BaseBuilder : IBaseExportBuilder<TransactionList>
    {
        public TransactionList TransactionList { get; protected set; }

        public abstract void Build(TransactionList transactionList);
        public abstract IEnumerable<string> GetResultAsLines();
        public abstract Stream GetResultAsStream();
        public abstract string GetResultAsString();
        public abstract byte[] GetResultAsBytes();

    }
}
