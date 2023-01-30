using System;

namespace Exthand.FinanceExports
{


    /// <summary>
    /// Represents a single bank transaction.
    /// </summary>
    public class Transaction
    {

        /// <summary>
        /// Unique Identifier of a transaction.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Unique sequence ID of this transaction
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Transaction's identifier provided by the bank.
        /// </summary>
        public string? RemoteId { get; set; }

        /// <summary>
        /// Transaction's date of creation.
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Transaction's date of execution.
        /// </summary>
        public DateTime? DateExecution { get; set; }

        /// <summary>
        /// Transaction's value date.
        /// </summary>
        public DateTime? DateValue { get; set; }

        /// <summary>
        /// Transaction's IBAN human readable name (eg 'Joanna's bank account')
        /// </summary>
        public string IBANName { get; set; }

        /// <summary>
        /// Transaction's IBAN Bank Account (eg 'BE93012345679')
        /// </summary>
        public string IBANAccount { get; set; }

        /// <summary>
        /// Transaction's bank name (eg 'ING')
        /// </summary>
        public string? Bank { get; set; }

        /// <summary>
        /// Transaction's amount in decimal format supporting positive or negative values.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction's currency (accepted current value is 'EUR')
        /// </summary>
        public string? Currency { get; set; } = "EUR";

        /// <summary>
        /// Counterpart's IBAN bank account (eg 'BE930123456789')
        /// </summary>
        public string? CounterpartIBAN { get; set; }

        /// <summary>
        /// Counterpart reference (eg 'CUST001')
        /// </summary>
        public string? CounterpartReference { get; set; }

        /// <summary>
        /// Counterpart Bank Identifier (BIC) (eg 'CUST001')
        /// </summary>
        public string? CounterpartBic { get; set; }

        /// <summary>
        /// Counterpart's name (eg 'Company 001')
        /// </summary>
        public string? CounterpartName { get; set; }

        /// <summary>
        /// ISO 20022 related DOMAIN code sets
        /// https://www.iso20022.org/catalogue-messages/additional-content-messages/external-code-sets 
        /// </summary>
        public string? Domain { get; set; }
        /// <summary>
        /// ISO 20022 related FAMILY code sets
        /// https://www.iso20022.org/catalogue-messages/additional-content-messages/external-code-sets 
        /// </summary>
        public string? Family { get; set; }
        /// <summary>
        /// ISO 20022 related SUBFAMILY code sets
        /// https://www.iso20022.org/catalogue-messages/additional-content-messages/external-code-sets 
        /// </summary>
        public string? SubFamily { get; set; }

        /// <summary>
        /// Transaction's remittance (description).
        /// </summary>
        public string? RemittanceUnstructured { get; set; }

        /// <summary>
        /// Transaction's structured information (For Belgian market only).
        /// </summary>
        public string? RemittanceStructuredRef { get; set; }
        /// <summary>
        /// Transaction's structured information (For Belgian market only).
        /// </summary>
        public string? RemittanceStructuredCode { get; set; }
        /// <summary>
        /// Transaction's structured information (For Belgian market only).
        /// </summary>
        public string? RemittanceStructuredIssuer { get; set; }

        /// <summary>
        /// Transaction's End2End identifier.
        /// </summary>
        public string? End2EndId { get; set; }


        public decimal? BalanceBefore { get; set; }
        public decimal? BalanceAfter { get; set; }
    }
}

