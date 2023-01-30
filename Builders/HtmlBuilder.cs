using Exthand.FinanceExports.Helpers;
using Exthand.FinanceExports.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Builders
{
    public class HtmlBuilder : BaseBuilder
    {
        public string Result { get; private set; }


        /// <summary>
        /// Build the content of the HTML file
        /// </summary>
        public override void Build(TransactionList transactionList)
        {
            TransactionList = transactionList;

            var nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = "",
                NumberDecimalDigits = 2
            };

            Result = EmbeddedResourceHelper.GetEmbeddedResource("Resources.codaHtml.html");
            Result = Result.Replace("{accountDescription}", TransactionList.IBANAccountDescription);
            Result = Result.Replace("{title}", $"CODA n°{TransactionList.transactionId} - {TransactionList.DateOfFirstTransaction:dd/MM/yyyy}");

            Result = Result.Replace("{openingBalanceDate}", TransactionList.DateOfFirstTransaction.ToString("dd/MM/yyyy"))
                .Replace("{openingBalanceAmount}", $"{TransactionList.BalanceOpening.ToString("F2", nfi)} {TransactionList.Currency}");
            
            Result = Result.Replace("{closingBalanceDate}", TransactionList.DateOfLastTransaction.ToString("dd/MM/yyyy"))
                .Replace("{closingBalanceDateAmount}", $"{TransactionList.BalanceClosing.ToString("F2", nfi)} {TransactionList.Currency}");

            if (TransactionList.transactions.Count() == 0)
            {
                Result = Result.Replace("{transactions}", string.Empty);
            }
            else
            {
                var htmlTransaction = EmbeddedResourceHelper.GetEmbeddedResource("Resources.codaHtmlTransaction.html");
                var stringBuilder = new StringBuilder();

                foreach (var transaction in TransactionList.transactions)
                {
                    var communication = transaction.RemittanceStructuredRef ?? transaction.RemittanceUnstructured;

                    var resultTransaction = htmlTransaction
                        .Replace("{transactionId}", $"{transaction.Id}")
                        .Replace("{amount}", $"{transaction.Amount.ToString("F2", nfi)} {transaction.Currency}")
                        .Replace("{executionDate}", transaction.DateExecution?.ToString("dd/MM/yyyy") ?? "N/A")
                        .Replace("{valueDate}", transaction.DateValue?.ToString("dd/MM/yyyy") ?? "N/A")
                        .Replace("{communication}", communication?.Replace("\n", "<br />") ?? "N/A");

                    stringBuilder.Append(resultTransaction);
                }

                Result = Result.Replace("{transactions}", stringBuilder.ToString());
            }

            Result = Result.Replace("{footer}", "<p>Opening and closing have been inferred from transactions.</p>");
        }

        public override IEnumerable<string> GetResultAsLines()
        {
            return new List<string>
            {
                Result
            };
        }

        public override Stream GetResultAsStream()
        {
            var memoryStream = new MemoryStream();
            memoryStream.Write(Encoding.UTF8.GetBytes(Result));
            return memoryStream;
        }

        public override string GetResultAsString()
        {
            return Result;
        }
    }
}
