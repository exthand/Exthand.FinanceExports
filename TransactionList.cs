using System;
using System.Collections.Generic;

namespace Exthand.FinanceExports
{


    /// <summary>
    /// Represents a single bank transaction.
    /// </summary>
    public class TransactionList
    {

        /// <summary>
        /// Unique identifier for this export (max length 16 CHAR).
        /// </summary>
        public string transactionId { get; set; }

        public string IBANAccount { get; set; }

        public string Currency { get; set; }

        public decimal BalanceOpening { get; set; }

        public decimal BalanceClosing { get; set; }

        public DateTime DateFirsTransaction { get; set; }

        public List<Transaction> transactions { get; set; }

    }
}

