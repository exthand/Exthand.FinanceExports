using Exthand.FinanceExports.Helpers;
using Exthand.FinanceExports.Models;
using Exthand.FinanceExports.Models.Coda;
using Exthand.FinanceExports.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exthand.FinanceExports.Builders
{
    public class Mt940Builder : BaseBuilder
    {
        public IList<string> Mt940Lines { get; private set; }

        private string NewLine { get; } = "\n";
        private Account _account;
        private DateTime _now;

        /// <summary>
        /// CTOR
        /// </summary>
        public Mt940Builder()
        {
            Mt940Lines = new List<string>();
        }


        /// <summary>
        /// Build the content of the MT940 file
        /// </summary>
        public override void Build(TransactionList transactionList)
        {
            TransactionList = transactionList;
            ResetBuilder();

            WriteHeader(TransactionList);
            WriteBalances(TransactionList);
            WriteTransactions(TransactionList);
            WriteFooter(TransactionList);
        }

        /// <summary>
        /// Get the content of the MT940 file as one string
        /// </summary>
        public override string GetResultAsString()
        {
            return string.Join('\n', Mt940Lines.Select(cl => cl.ToString()));
        }

        /// <summary>
        /// Get the content of the MT940 file as a collection of strings
        /// </summary>
        public override IEnumerable<string> GetResultAsLines()
        {
            return Mt940Lines.Select(cl => cl.ToString());
        }

        /// <summary>
        /// Get the content of the MT940 file in a stream
        /// </summary>
        public override Stream GetResultAsStream()
        {
            var memoryStream = new MemoryStream();

            foreach (var mt940Line in Mt940Lines)
            {
                memoryStream.Write(Encoding.UTF8.GetBytes(mt940Line.ToString() + "\n"));
            }

            return memoryStream;
        }


        #region Private methods

        /// <summary>
        /// Reset all internal properties in case the builder is reused
        /// </summary>
        private void ResetBuilder()
        {
            Mt940Lines.Clear();
            _account = null;
            _now = DateTime.Now;
        }

        /// <summary>
        /// Writes header in the transacions collection.
        /// </summary>
        private void WriteHeader(TransactionList transactionList)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($":20:{transactionList.transactionId}{NewLine}");
            stringBuilder.Append($":25:{transactionList.IBANAccount}{transactionList.Currency}{NewLine}");
            stringBuilder.Append($":28C:00001{NewLine}");
            Mt940Lines.Add(stringBuilder.ToString());
            return;
}


        /// <summary>
        /// Writes balance info.
        /// </summary>
        private void WriteBalances(TransactionList transactionList)
        {
            var stringBuilder = new StringBuilder();
            if (transactionList.BalanceOpening >= 0)
            { stringBuilder.Append($":60F:C{transactionList.DateOfFirstTransaction:yyMMdd}{transactionList.Currency}{Math.Abs(transactionList.BalanceOpening):0.00}{NewLine}"); }
            else
            { stringBuilder.Append($":60F:D{transactionList.DateOfFirstTransaction:yyMMdd}{transactionList.Currency}{Math.Abs(transactionList.BalanceOpening):0.00}{NewLine}"); }
            Mt940Lines.Add(stringBuilder.ToString());
            return;
        }

        /// <summary>
        /// Writes all transactions in the transacions collection.
        /// </summary>
        private void WriteTransactions(TransactionList transactionList)
        {
            var stringBuilder = new StringBuilder();
            foreach (Transaction transaction in transactionList.transactions)
            {
                stringBuilder.Append($"N099//{Clean(transaction.RemittanceUnstructured, 16)}{NewLine}");
                stringBuilder.Append($":61:{transaction.DateValue.Value:yyMMdd}{transaction.DateExecution.Value:MMdd}{GetMoney(transaction.Amount)}{NewLine}");
                stringBuilder.Append($":86:{NewLine}");
            }
            Mt940Lines.Add(stringBuilder.ToString());
            return;
        }

        /// <summary>
        /// Writes all footer info.
        /// </summary>
        private void WriteFooter(TransactionList transactionList)
        {
            var stringBuilder = new StringBuilder();
            if (transactionList.BalanceClosing >= 0)
            { stringBuilder.Append($":62F:C{transactionList.DateOfLastTransaction:yyMMdd}{transactionList.Currency}{Math.Abs(transactionList.BalanceClosing).ToString("0.00")}{NewLine}"); }
            else
            { stringBuilder.Append($":62F:D{transactionList.DateOfLastTransaction:yyMMdd)}{transactionList.Currency}{Math.Abs(transactionList.BalanceClosing).ToString("0.00")}{NewLine}"); }
            Mt940Lines.Add(stringBuilder.ToString());
            return;
        }

        private string Clean(string text, int length = 9999)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (length == 9999 || (text.Length < length))
                    return text.Replace("\n", " - ");
                else
                    return text.Replace("\n", " - ").Substring(0, length);
            }
            else
                return "";
        }

        private string GetMoney(decimal amount)
        {
            if (amount >= 0)
                return "C" + Math.Abs(amount).ToString("0.00");
            return "D" + Math.Abs(amount).ToString("0.00");
        }


        #endregion
    }
}
