using System;
using System.Collections.Generic;

namespace Exthand.FinanceExports.Models.Coda
{
    public class CodaValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public CodaValidationException(string message, IEnumerable<string> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
