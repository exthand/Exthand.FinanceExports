using Exthand.FinanceExports.Helpers;
using Exthand.FinanceExports.Models;
using Exthand.FinanceExports.Models.Coda;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

//TODO: Suppot non Belgian IBANS, add VAT

namespace Exthand.FinanceExports.Builders
{
    public class CodaBuilder : BaseBuilder
    {
        public IList<ICodaLineType> CodaLines { get; private set; }

        private Account _account;
        private DateTime _now;

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

            _account = CodaAccountHelper.ParseAccount(transactionList.IBANAccount, transactionList.Currency);

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
            CodaLines.Clear();
            _account = null;
            _now = DateTime.Now;
        }

        /// <summary>
        /// Generate line for Header
        /// </summary>
        private void GenerateCodaHeader()
        {
            CodaLines.Add(new CodaLineType0
            {
                CreationDate = _now.Date,
                BankIdentificationNumber = 0,
                FileReference = null,
                AccountHolderName = TransactionList.IBANAccountDescription,
                Bic = "", // TODO
                CompanyNumber = 0,
                SeparateApplication = 0,
                TransactionReference = null,
                RelatedReference = null
            });
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
                //TODO: JG : update seq number.
                SequenceNumber = 0,
                StatementSequenceNumber = 0
                //SequenceNumber = Request.SequenceNumber,
                //StatementSequenceNumber = Request.SequenceNumber
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
                BalanceDate = TransactionList.DateOfLastTransaction.Value,
                SequenceNumber = 0,
                //TODO: JG : update seq number.
                //SequenceNumber = Request.SequenceNumber,
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
                CreditMovement = TransactionList.transactions.Where(tr => tr.Amount > 0).Select(tr => tr.Amount).Sum(),
                DebitMovement = TransactionList.transactions.Where(tr => tr.Amount < 0).Select(tr => tr.Amount).Sum(),
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
                Communication = "EXTHAND.COM CODA GENERATOR V0.2",
                LinkCode = true
            });

            CodaLines.Add(new CodaLineType4
            {
                SequenceNumber = 1,
                DetailNumber = 1,
                Communication = $"AccountID: {TransactionList.IBANAccount}",
                LinkCode = true
            });

        }

        /// <summary>
        /// Generate Transaction Records
        /// </summary>
        private void GenerateTransactionRecords()
        {
            var contSeqNbr = 1;
            
            foreach (var transaction in TransactionList.transactions)
            {
                var communication = transaction.RemittanceStructuredRef ?? transaction.RemittanceUnstructured ?? (transaction.RemittanceUnstructured ?? "").Replace("\n", " ");
                var hasDetailedCommunication = transaction.RemittanceUnstructured != null && (transaction.RemittanceUnstructured.Length > 53 || transaction.RemittanceUnstructured.Contains("\n"));
                var communicationType = transaction.RemittanceUnstructured != null
                    ? CommunicationType.Structured
                    : CommunicationType.Unstructured;
                
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
                Communication = communication,
                EntryDate = transaction.DateExecution,
                SequenceNumberStatement = 1,
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
                    LinkCode = i < (splittedCommunication.Length - 1)
                });

                detailNumber++;
            }
        }

        #endregion
    }
}
