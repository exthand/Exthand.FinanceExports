namespace Exthand.FinanceExports.Models.Coda
{
    public interface ICodaLineType
    {
        CodaLineType GetCodaLineType();

        string ToString();
        void Validate();        
    }
}
