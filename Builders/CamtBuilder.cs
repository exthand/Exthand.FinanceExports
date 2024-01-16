using Exthand.FinanceExports.Helpers;
using Exthand.FinanceExports.Models;
using Exthand.FinanceExports.Models.Camt;
using Exthand.FinanceExports.Models.Coda;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Serialization;

namespace Exthand.FinanceExports.Builders
{
    public class CamtBuilder : BaseBuilder
    {
        public Document CamtDocument { get; private set; }

        private Guid _id;
        private DateTime _today;


        /// <summary>
        /// Build the content of the CAMT file
        /// </summary>
        public override void Build(TransactionList transactionList)
        {
            TransactionList = transactionList;
            ResetBuilder();

            CamtDocument = new Document
            {
                BkToCstmrStmt = new BankToCustomerStatementV03
                {
                    GrpHdr = GetHeader(),
                    Stmt = new AccountStatement3[] { GetStatement() }
                }
            };
        }

        /// <summary>
        /// Operation not supported
        /// </summary>
        public override IEnumerable<string> GetResultAsLines()
        {
            throw new NotSupportedException("Operation not supported");
        }

        /// <summary>
        /// Get the content of the CAMT file in a stream
        /// </summary>
        public override Stream GetResultAsStream()
        {
            var memoryStream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(Document));
            serializer.Serialize(memoryStream, CamtDocument);
            return memoryStream;
        }

        /// <summary>
        /// Get the content of the CAMT file as one string
        /// </summary>
        public override string GetResultAsString()
        {
            using (var stringwriter = new ExtentedStringWriter(Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof(Document));
                serializer.Serialize(stringwriter, CamtDocument);
                return stringwriter.ToString();
            }
        }

        #region Private Methods

        /// <summary>
        /// Reset all internal properties in case the builder is reused
        /// </summary>
        private void ResetBuilder()
        {
            CamtDocument = null;
            _id = Guid.NewGuid();
            _today = DateTime.Today;
        }

        private GroupHeader58 GetHeader()
        {
            return new GroupHeader58
            {
                MsgId = _id.ToString("N").Substring(0, 20),
                CreDtTm = _today,
                MsgPgntn = new Pagination
                {
                    PgNb = "1",
                    LastPgInd = true
                },
                AddtlInf = "camt.053.001.03"
            };
        }

        private AccountStatement3 GetStatement()
        {
            return new AccountStatement3
            {
                Id = TransactionList.TransactionId + "-000001",
                ElctrncSeqNb = 0,
                ElctrncSeqNbSpecified = true,
                LglSeqNb = 0,
                LglSeqNbSpecified = true,
                CreDtTm = _today,
                FrToDt = new DateTimePeriodDetails
                {
                    FrDtTm = TransactionList.DateOfFirstTransaction.Value,
                    ToDtTm = TransactionList.DateOfLastTransaction.Value
                },
                Acct = new CashAccount25
                {
                    Id = new AccountIdentification4Choice
                    {
                        Item = TransactionList.IBANAccount
                    },
                    Ccy = TransactionList.Currency
                },
                Bal = GetBalances().ToArray(),
                TxsSummry = GetTransactionSummary(),
                Ntry = GetEntries().ToArray()
            };
        }

        private IEnumerable<CashBalance3> GetBalances()
        {
            return new List<CashBalance3>
            {
                GetBalance(new Balance(){Amount = TransactionList.BalanceOpening, Computed = true, Currency = TransactionList.Currency, ReferenceDate = TransactionList.DateOfFirstTransaction.Value}, true),
                GetBalance(new Balance(){Amount = TransactionList.BalanceClosing, Computed = true, Currency = TransactionList.Currency, ReferenceDate = TransactionList.DateOfLastTransaction.Value}, false)
            };
        }

        private CashBalance3 GetBalance(Balance balance, bool isOpeningBalance)
        {
            return new CashBalance3
            {
                Tp = new BalanceType12
                {
                    CdOrPrtry = new BalanceType5Choice
                    {
                        Item = isOpeningBalance ? BalanceType12Code.OPBD : BalanceType12Code.CLBD
                    }
                },
                Amt = new ActiveOrHistoricCurrencyAndAmount
                {
                    Ccy = balance?.Currency ?? "",
                    Value = Math.Abs(balance?.Amount ?? 0 )
                },
                CdtDbtInd = (balance?.Amount ?? 0) >= 0 ? CreditDebitCode.CRDT : CreditDebitCode.DBIT,
                Dt = new DateAndDateTimeChoice
                {
                    Item = balance?.ReferenceDate ?? (isOpeningBalance ? TransactionList.DateOfFirstTransaction.Value.AddDays(-1) : TransactionList.DateOfLastTransaction.Value)
                }
            };
        }


        private TotalTransactions2 GetTransactionSummary()
        {
            var sum = TransactionList.Transactions.Sum(t => t.Amount);

            var summary = new TotalTransactions2
            {
                TtlNtries = new NumberAndSumOfTransactions2
                {
                    NbOfNtries = $"{TransactionList.Transactions.Count()}",
                    Sum = Math.Abs(sum),
                    SumSpecified = true,
                    TtlNetNtryAmt = Math.Abs(sum),
                    TtlNetNtryAmtSpecified = true,
                    CdtDbtInd = sum >= 0 ? CreditDebitCode.CRDT : CreditDebitCode.DBIT,
                    CdtDbtIndSpecified = true
                }
            };

            var sumCredit = TransactionList.Transactions.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            summary.TtlCdtNtries = new NumberAndSumOfTransactions1
            {
                NbOfNtries = $"{TransactionList.Transactions.Where(t => t.Amount >= 0).Count()}",
                Sum = Math.Abs(sumCredit),
                SumSpecified = true
            };
            
            var sumDebit = TransactionList.Transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
            summary.TtlDbtNtries = new NumberAndSumOfTransactions1
            {
                NbOfNtries = $"{TransactionList.Transactions.Where(t => t.Amount < 0).Count()}",
                Sum = Math.Abs(sumDebit),
                SumSpecified = true
            };
            
            return summary;
        }

        private IEnumerable<ReportEntry3> GetEntries()
        {
            return TransactionList.Transactions.Select(GetEntry);
        }

        private ReportEntry3 GetEntry(Transaction transaction)
        {
            var entry = new ReportEntry3
            {
                Amt = new ActiveOrHistoricCurrencyAndAmount
                {
                    Ccy = transaction.Currency,
                    Value = Math.Abs(transaction.Amount)
                },
                CdtDbtInd = transaction.Amount >= 0 ? CreditDebitCode.CRDT : CreditDebitCode.DBIT,
                RvslInd = false,
                RvslIndSpecified = true,
                Sts = EntryStatus2Code.BOOK,
                AcctSvcrRef = transaction.IBANAccount,
                BkTxCd = GetBankTransactionCode(transaction.Amount >= 0),
                NtryDtls = new EntryDetails2[]
                {
                    new EntryDetails2
                    {
                        TxDtls = new EntryTransaction3[]
                        {
                            GetTransactionDetails(transaction)
                        }
                    }
                }
            };

            if (transaction.DateExecution.HasValue)
            {
                entry.BookgDt = new DateAndDateTimeChoice
                {
                    Item = transaction.DateExecution.Value
                };
            }

            if (transaction.DateValue.HasValue)
            {
                entry.ValDt = new DateAndDateTimeChoice
                {
                    Item = transaction.DateValue.Value
                };
            }

            return entry;
        }

        private EntryTransaction3 GetTransactionDetails(Transaction transaction)
        {
            var details = new EntryTransaction3
            {
                Refs = new TransactionReferences3
                {
                    MsgId = transaction.Sequence.ToString(),
                    TxId = transaction.Sequence.ToString(),
                    EndToEndId = transaction.End2EndId
                },
                AmtDtls = new AmountAndCurrencyExchange3
                {
                    CntrValAmt = new AmountAndCurrencyExchangeDetails3
                    {
                        Amt = new ActiveOrHistoricCurrencyAndAmount
                        {
                            Ccy = transaction.Currency,
                            Value = Math.Abs(transaction.Amount)
                        }
                    }
                },
                BkTxCd = GetBankTransactionCode(transaction.Amount >= 0)                
            };

            if (!string.IsNullOrEmpty(transaction.CounterpartName) || !string.IsNullOrEmpty(transaction.CounterpartReference))
            {
                details.RltdPties = new TransactionParties3();

                if (!string.IsNullOrEmpty(transaction.CounterpartName))
                {
                    if (transaction.Amount >= 0)
                    {
                        details.RltdPties.Dbtr = new PartyIdentification43
                        {
                            Nm = transaction.CounterpartName
                        };
                    }
                    else
                    {
                        details.RltdPties.Cdtr = new PartyIdentification43
                        {
                            Nm = transaction.CounterpartName
                        };
                    }                    
                }

                if (!string.IsNullOrEmpty(transaction.CounterpartReference))
                {
                    if (transaction.Amount >= 0)
                    {
                        details.RltdPties.DbtrAcct = new CashAccount24
                        {
                            Id = new AccountIdentification4Choice
                            {
                                Item = transaction.CounterpartReference
                            }
                        };
                    }
                    else
                    {
                        details.RltdPties.CdtrAcct = new CashAccount24
                        {
                            Id = new AccountIdentification4Choice
                            {
                                Item = transaction.CounterpartReference
                            }
                        };
                    }                        
                }
            }

            if (!string.IsNullOrEmpty(transaction.CounterpartBic))
            {
                details.RltdAgts = new TransactionAgents3();

                if (transaction.Amount >= 0)
                {
                    details.RltdAgts.DbtrAgt = new BranchAndFinancialInstitutionIdentification5
                    {
                        FinInstnId = new FinancialInstitutionIdentification8
                        {
                            BICFI = transaction.CounterpartBic
                        }
                    };
                }
                else
                {
                    details.RltdAgts.CdtrAgt = new BranchAndFinancialInstitutionIdentification5
                    {
                        FinInstnId = new FinancialInstitutionIdentification8
                        {
                            BICFI = transaction.CounterpartBic
                        }
                    };
                }
            }

            if (!string.IsNullOrEmpty(transaction.RemittanceStructuredRef) || !string.IsNullOrEmpty(transaction.RemittanceUnstructured))
            {
                details.RmtInf = new RemittanceInformation7();

                if (!string.IsNullOrEmpty(transaction.RemittanceUnstructured))
                {
                    details.RmtInf.Ustrd = new string[]
                        { transaction.RemittanceUnstructured.Replace('\r','_').Replace('\n','_') };

                    // This has been added for KYRIBA compliance reasons.
                    details.Refs.MsgId = details.RmtInf.Ustrd[0];
                }

                if (!string.IsNullOrEmpty(transaction.RemittanceStructuredRef))
                {
                    details.RmtInf.Strd = new StructuredRemittanceInformation9[]
                    {
                        new StructuredRemittanceInformation9
                        {
                            AddtlRmtInf = new string[] { transaction.RemittanceStructuredRef }
                        }
                    };
                    details.Refs.MsgId = details.RmtInf.Strd[0].AddtlRmtInf[0];
                }

            }


            return details;
        }

        private BankTransactionCodeStructure4 GetBankTransactionCode(bool isCredit)
        {
            // https://www.nordea.com/Images/36-295405/CAAR_camt.053.001.02_Account%20Statement_Extended_v_1.3.pdf

            return new BankTransactionCodeStructure4
            {
                Domn = new BankTransactionCodeStructure5
                {
                    Cd = "PMNT",
                    Fmly = new BankTransactionCodeStructure6
                    {
                        Cd = isCredit ? "RCDT" : "ICDT",
                        SubFmlyCd = "ESCT"
                    }
                },
                //Prtry = new ProprietaryBankTransactionCodeStructure1
                //{
                //    Cd = isCredit ? "0150000" : "0101000",
                //    Issr = "BBA"
                //}
            };
        }

        #endregion
    }
}
