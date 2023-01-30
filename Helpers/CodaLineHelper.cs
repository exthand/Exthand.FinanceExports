using Exthand.FinanceExports.Models.Coda;

namespace Exthand.FinanceExports.Helpers
{
    public static class CodaLineHelper
    {
        public static bool IsRecordLine(CodaLineType codaLineType)
        {
            switch (codaLineType)
            {
                case CodaLineType.OldBalance:
                case CodaLineType.TransactionPart1:
                case CodaLineType.TransactionPart2:
                case CodaLineType.TransactionPart3:
                case CodaLineType.InformationPart1:
                case CodaLineType.InformationPart2:
                case CodaLineType.InformationPart3:
                case CodaLineType.NewBalance:
                    return true;
                default:
                    return false;
            }
        }
    }
}
