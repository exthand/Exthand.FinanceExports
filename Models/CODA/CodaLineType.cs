namespace Exthand.FinanceExports.Models.Coda
{
    public enum CodaLineType
    {
        /// <summary>
        /// The header record (0)
        /// </summary>
        Header = 0,

        /// <summary>
        /// The old balance record (1)
        /// </summary>
        OldBalance = 1,

        /// <summary>
        /// The first movement record (21)
        /// </summary>
        TransactionPart1 = 21,

        /// <summary>
        /// The second movement record (22)
        /// </summary>
        TransactionPart2 = 22,

        /// <summary>
        /// The thrid movement record (23)
        /// </summary>
        TransactionPart3 = 23,

        /// <summary>
        /// The first information record (31)
        /// </summary>
        InformationPart1 = 31,

        /// <summary>
        /// The second information record (32)
        /// </summary>
        InformationPart2 = 32,

        /// <summary>
        /// The third information record (33)
        /// </summary>
        InformationPart3 = 33,

        /// <summary>
        /// The free communication record (4)
        /// </summary>
        Message = 4,

        /// <summary>
        /// The new balance record (8)
        /// </summary>
        NewBalance = 8,

        /// <summary>
        /// The trailer record (9)
        /// </summary>
        Footer = 9
    }
}
