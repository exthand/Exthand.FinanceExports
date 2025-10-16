    using Exthand.FinanceExports.Helpers;
using Exthand.FinanceExports.Models;
using Exthand.FinanceExports.Models.Coda;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Exthand.FinanceExports.Builders
{
    public class CodaBuilder : BaseBuilder
    {
        public IList<ICodaLineType> CodaLines { get; private set; }

        private Account _account;
        private DateTime _now;
        private int _sequenceNumber;

        /// <summary>
        /// CTOR
        /// </summary>
        public CodaBuilder()
        {
            CodaLines = new List<ICodaLineType>();
        }


        /// <summary>
        /// Build the content of the CODA file
        /// </summary>
        public override void Build(TransactionList transactionList)
        {
            TransactionList = transactionList;
            ResetBuilder();

            // We initialize the sequence number.
            if (transactionList.Transactions.First().StatementNumber == 0)
            {
                _sequenceNumber = TransactionList.DateOfFirstTransaction.Value.DayOfYear;
            }
            else
            {
                _sequenceNumber = transactionList.Transactions.First().StatementNumber;
            }
            _account = CodaAccountHelper.ParseAccount(transactionList.IBANAccount, transactionList.Currency);

            // We set as the reation date of the file at the same datetime as the first transaction.
            _now = transactionList.Transactions.First().DateValue.HasValue? transactionList.Transactions.First().DateValue.Value : DateTime.Now;
            
            GenerateCodaHeader();
            GenerateOpeningBalanceRecord();
            GenerateTransactionRecords();
            GenerateClosingBalanceRecord();
            GenerateDebugRecords();
            GenerateCodaFooter();
        }

        /// <summary>
        /// Get the content of the CODA file as one string
        /// </summary>
        public override string GetResultAsString()
        {
            return string.Join(Environment.NewLine, CodaLines.Select(cl => cl.ToString()));
        }

        /// <summary>
        /// Get the content of the CODA file as a collection of strings
        /// </summary>
        public override IEnumerable<string> GetResultAsLines()
        {
            return CodaLines.Select(cl => cl.ToString());
        }

        /// <summary>
        /// Get the content of the CODA file in a stream
        /// </summary>
        public override Stream GetResultAsStream()
        {
            var memoryStream = new MemoryStream();

            foreach (var codaLine in CodaLines)
            {
                memoryStream.Write(Encoding.UTF8.GetBytes(codaLine.ToString() + Environment.NewLine));
            }

            return memoryStream;
        }


        #region Private methods

        /// <summary>
        /// Reset all internal properties in case the builder is reused
        /// </summary>
        private void ResetBuilder()
        {
            CodaLines = new List<ICodaLineType>();
            _account = null;
        }

        /// <summary>
        /// Generate line for Header
        /// </summary>
        private void GenerateCodaHeader()
        {
            CodaLineType0 codaLineType0 = new CodaLineType0();
            codaLineType0.CreationDate = _now.Date;
            codaLineType0.BankIdentificationNumber = 0;
            codaLineType0.FileReference = null;
            codaLineType0.AccountHolderName = TransactionList.IBANAccountDescription;
            codaLineType0.Bic = "";
            codaLineType0.CompanyNumber = TransactionList.CompanyVAT;
            codaLineType0.SeparateApplication = 0;
            codaLineType0.TransactionReference = null;
            codaLineType0.RelatedReference = null;
            
            CodaLines.Add(codaLineType0);
            
        }


        /// <summary>
        /// Generate line for Opening Balance Record
        /// </summary>
        private void GenerateOpeningBalanceRecord() 
        {
            CodaLines.Add(new CodaLineType1
            {
                Account = _account,
                AccountDescription = TransactionList.IBANAccount,
                AccountHolderName = TransactionList.IBANAccountDescription,
                Balance = TransactionList.BalanceOpening,
                BalanceDate = TransactionList.DateOfFirstTransaction.Value,
                SequenceNumber = _sequenceNumber,
                StatementSequenceNumber = _sequenceNumber
            });
        }


        /// <summary>
        /// Generate line for Closing Balance Record
        /// </summary>
        private void GenerateClosingBalanceRecord()
        {
            CodaLines.Add(new CodaLineType8
            {
                Account = _account,
                Balance = TransactionList.BalanceClosing,
                // We take dateoffirsttransaction because it's the closing of the same.
                BalanceDate = TransactionList.DateOfLastTransaction.Value.AddDays(-1),
                SequenceNumber = _sequenceNumber,
                LinkCode = false
            });
        }


        /// <summary>
        /// Generate lines for New Balance, Message(s) and Footer Records
        /// </summary>
        private void GenerateCodaFooter()
        {
            CodaLines.Add(new CodaLineType9
            {
                NumberOfRecors = CodaLines.Count(l => CodaLineHelper.IsRecordLine(l.GetCodaLineType())),
                CreditMovement = TransactionList.Transactions.Where(tr => tr.Amount > 0).Select(tr => tr.Amount).Sum(),
                DebitMovement = TransactionList.Transactions.Where(tr => tr.Amount < 0).Select(tr => tr.Amount).Sum(),
                MultipleFiles = false
            });
        }

        /// <summary>
        /// Generate Debug Records
        /// </summary>
        private void GenerateDebugRecords()
        {
            CodaLines.Add(new CodaLineType4
            {
                SequenceNumber = 1,
                DetailNumber = 0,
                Communication = $"EXTHAND.COM CODA GENERATOR V1.2.0 {DateTime.UtcNow.ToString("yyyyMMddHHmmss")}",
                LinkCode = true
            });

            CodaLines.Add(new CodaLineType4
            {
                SequenceNumber = 1,
                DetailNumber = 1,
                Communication = $"AccountID: {TransactionList.IBANAccount}",
                LinkCode = false
            });

        }

        /// <summary>
        /// Generate Transaction Records
        /// </summary>
        private void GenerateTransactionRecords()
        {
            var contSeqNbr = 1;
            
            foreach (var transaction in TransactionList.Transactions.Where(t=>t.StatementType is null))
            {
                string communication = "";
                if (transaction.RemittanceStructuredRef is not null)
                    communication = transaction.RemittanceStructuredRef;
                else
                {
                    if (transaction.RemittanceUnstructured is not null)
                        communication = transaction.RemittanceUnstructured;
                    else
                        communication = "-";
                }
                bool hasDetailedCommunication = transaction.RemittanceUnstructured != null && (transaction.RemittanceUnstructured.Length > 53 || transaction.RemittanceUnstructured.Contains("\n"));
                bool hasURL = !(string.IsNullOrEmpty(transaction.URLExtended)) ? true : false;
                var communicationType = transaction.RemittanceUnstructured != null
                    ? CommunicationType.Unstructured
                    : CommunicationType.Structured;
                
                GenerateTransactionRecord21(transaction, contSeqNbr, communication, communicationType, false, true);
                bool gotCounterpart = !string.IsNullOrEmpty(transaction.CounterpartName) ||
                             !string.IsNullOrEmpty(transaction.CounterpartReference);
                GenerateTransactionRecord22(transaction, contSeqNbr, communication, hasDetailedCommunication && !gotCounterpart, gotCounterpart);
                if (gotCounterpart)
                {
                    GenerateTransactionRecord23(transaction, contSeqNbr, communication, hasDetailedCommunication);
                }

                if (hasDetailedCommunication)
                    GenerateTransactionRecords31(transaction, contSeqNbr);

                contSeqNbr++;
            }
        }

        /// <summary>
        /// Add one CodaLineType21 to the current Coda file
        /// </summary>
        private void GenerateTransactionRecord21(Transaction transaction, int contSeqNbr, string communication, CommunicationType communicationType,  bool hasDetailedCommunication, bool nextCode)
        {
            // Transaction 1
            CodaLines.Add(new CodaLineType21
            {
                ContinuousSequenceNumber = contSeqNbr % 10000,
                DetailNumber = 0,
                BankReferenceNumber = null,
                Amount = transaction.Amount,
                ValueDate = transaction.DateValue,
                TransactionCode = 0,
                CommunicationType = communicationType,
                StructuredCommunicationType = communicationType == CommunicationType.Structured ? "101" : "",
                Communication = communication,
                EntryDate = transaction.DateExecution,
                SequenceNumberStatement = TransactionList.DateOfFirstTransaction.Value.DayOfYear,
                NextCode = nextCode,
                LinkCode = hasDetailedCommunication
            });
        }
        
        /// <summary>
        /// Add one CodaLineType22 to the current Coda file
        /// </summary>
        private void GenerateTransactionRecord22(Transaction transaction, int contSeqNbr, string communication, bool hasDetailedCommunication, bool nextCode)
        {
            // Transaction 1
            CodaLines.Add(new CodaLineType22
            {
                ContinuousSequenceNumber = contSeqNbr % 10000,
                DetailNumber = 0,
                Communication = communication,
                NextCode = nextCode,
                LinkCode = hasDetailedCommunication,
                TransactionType = TransactionType.Unknown,
                CounterpartyBic = ""
            });
        }

        /// <summary>
        /// Add one CodaLineType23 to the current Coda file
        /// </summary>
        private void GenerateTransactionRecord23(Transaction transaction, int contSeqNbr, string communication, bool hasDetailedCommunication)
        {
            // Transaction 1
            CodaLines.Add(new CodaLineType23
            {
                ContinuousSequenceNumber = contSeqNbr % 10000,
                CounterpartyAccount = CodaAccountHelper.ParseAccount(transaction.CounterpartReference, transaction.Currency),
                CounterpartyName = transaction.CounterpartName,
                DetailNumber = 0,
                Communication = communication,
                LinkCode = hasDetailedCommunication,
            });
        }

        /// <summary>
        /// Add one or many CodaLineType31 to the current Coda file
        /// </summary>
        private void GenerateTransactionRecords31(Transaction transaction, int contSeqNbr)
        {
            var detailNumber = 1;

            var splittedCommunication = transaction.RemittanceUnstructured.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < splittedCommunication.Length; i++)
            {
                CodaLines.Add(new CodaLineType31
                {
                    ContinuousSequenceNumber = contSeqNbr % 10000,
                    DetailNumber = detailNumber % 10000,
                    Communication = splittedCommunication[i],
                    BankReferenceNumber = null,
                    CommunicationType = CommunicationType.Unstructured,
                    NextCode = false,
                    LinkCode = (i < (splittedCommunication.Length - 1) || !string.IsNullOrEmpty(transaction.URLExtended))
                });

                detailNumber++;
            }

            if (!string.IsNullOrEmpty(transaction.URLExtended))
            {
                CodaLines.Add(new CodaLineType31
                {
                    ContinuousSequenceNumber = contSeqNbr % 10000,
                    DetailNumber = detailNumber % 10000,
                    Communication = transaction.URLExtended,
                    BankReferenceNumber = null,
                    CommunicationType = CommunicationType.Unstructured,
                    NextCode = false,
                    LinkCode = false
                });
            }

        }

        #endregion
    }
}
