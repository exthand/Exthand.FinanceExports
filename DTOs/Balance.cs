using System;

namespace Exthand.FinanceExports.Models
{
    public class Balance
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public bool Computed { get; set; }
    }
}
