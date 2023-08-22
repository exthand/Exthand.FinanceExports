using System;

namespace Exthand.FinanceExports.Helpers
{
    public static class CodaStringHelper
    {
        /// <summary>
        /// Print a boolean in CODA format
        /// </summary>
        public static string FormatBoolean(bool value)
        {
            return value ? "1" : "0";
        }


        /// <summary>
        /// Print an amount in the CODA format
        /// </summary>
        /// <param name="value">Amount</param>
        /// <param name="maxChars">Number of chars for the amount (does not include sign)</param>
        /// <param name="paddingChar">Padding char</param>
        /// <param name="includeSign">Include leading sign</param>
        public static string FormatBalance(decimal amount, int maxChars, char paddingChar = '0', bool includeSign = true)
        {
            var value = amount.ToString("#.000")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("-", "")
                .PadLeft(maxChars, paddingChar);

            if (!includeSign)
                return value;

            var sign = amount < 0 ? "1" : "0";
            return $"{sign}{value}";
        }


        /// <summary>
        /// Truncate a string
        /// </summary>
        public static string Truncate(string value, int maxChars)
        {
            value = value.Replace('\r', '_').Replace('\n', '_');
            return value.Length <= maxChars ? value : value.Substring(0, maxChars);
        }


        /// <summary>
        /// Trucate the string if too long, otherwise pad it to the left to maxChars len
        /// </summary>
        public static string TrucateOrPadLeft(string value, int maxChars, char paddingChar = ' ')
        {
            if (value == null)
                return new string(paddingChar, maxChars);

            value = value.Replace('\r', '_').Replace('\n', '_');

            if (value.Length == maxChars)
                return value;

            if (value.Length > maxChars)
                return value.Substring(0, maxChars);

            return value.PadLeft(maxChars, paddingChar);
        }


        /// <summary>
        /// Truncate the string is too long, otherwise pad it to the right to maxChars len
        /// </summary>
        public static string TrucateOrPadRight(string value, int maxChars, char paddingChar = ' ')
        {
            if (value == null)
                return new string(paddingChar, maxChars);

            value = value.Replace('\r', '_').Replace('\n', '_');

            if (value.Length == maxChars)
                return value;

            if (value.Length > maxChars)
                return value.Substring(0, maxChars);

            return value.PadRight(maxChars, paddingChar);
        }

        public static string Substring(string value, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(value) || startIndex < 0 || length < 0)
            {
                return string.Empty;
            }

            value = value.Replace('\r', '_').Replace('\n', '_');

            var valueLength = value.Length;

            if (startIndex >= valueLength)
            {
                return string.Empty;
            }

            if (startIndex + length >= valueLength)
            {
                return value.Substring(startIndex);
            }

            return value.Substring(startIndex, length);
        }
    }
}
