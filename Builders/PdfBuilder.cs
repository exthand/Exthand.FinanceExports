using Exthand.FinanceExports.Helpers;
using Exthand.FinanceExports.Models;
using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Builders
{
    public class PdfBuilder : BaseBuilder
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

            Result = Result.Replace("{openingBalanceDate}", TransactionList.DateOfFirstTransaction.Value.ToString("dd/MM/yyyy"))
                .Replace("{openingBalanceAmount}", $"{TransactionList.BalanceOpening.ToString("F2", nfi)} {TransactionList.Currency}");
            
            Result = Result.Replace("{closingBalanceDate}", TransactionList.DateOfLastTransaction.Value.ToString("dd/MM/yyyy"))
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

            Result = Result.Replace("{footer}", "");
        }

        public override IEnumerable<string> GetResultAsLines()
        {
            throw new NotImplementedException();
        }

        public override Stream GetResultAsStream()
        {
            throw new NotImplementedException();
        }

        public override byte[] GetResultAsBytes()
        {
            var memoryStream = new MemoryStream();
            ConverterProperties converterProperties = new();
            PdfWriter pdfWriter = new PdfWriter(memoryStream);
            pdfWriter.SetCloseStream(false);
            PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            pdfDocument.SetDefaultPageSize(PageSize.A4);
            HtmlConverter.ConvertToPdf(Result, pdfDocument, converterProperties);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        public override string GetResultAsString()
        {
            throw new NotImplementedException();
        }
    }
}
