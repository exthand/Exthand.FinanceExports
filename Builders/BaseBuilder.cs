using Exthand.FinanceExports.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Exthand.FinanceExports.Builders
{
    public abstract class BaseBuilder : IBaseExportBuilder<TransactionList>
    {
        public TransactionList TransactionList { get; protected set; }
        // TODO : JG : Remove
        //public ExportTestOptions TestOptions { get; set; }

        public abstract void Build(TransactionList transactionList);
        public abstract IEnumerable<string> GetResultAsLines();
        public abstract Stream GetResultAsStream();
        public abstract string GetResultAsString();


        //TODO: Ensure we don't need anymore this.
        /// <summary>
        /// Make sure balances make sense
        /// </summary>
        //protected void SanitizeBalances()
        //{
        //    OpeningBalance = new Balance
        //    {
        //        Amount = Request.OpeningBalance?.Amount ?? 0,
        //        Currency = Request.OpeningBalance?.Currency ?? "EUR",
        //        ReferenceDate = Request.OpeningBalance?.ReferenceDate ?? Request.DateFrom,
        //        Computed = Request.OpeningBalance == null
        //    };

        //    // Compute closing balance
        //    ClosingBalance = new Balance
        //    {
        //        Amount = OpeningBalance.Amount + Request.Transactions.Select(t => t.Amount).Sum(),
        //        Currency = OpeningBalance.Currency,
        //        ReferenceDate = Request.DateTo,
        //        Computed = true
        //    };
        //}
    }
}
