namespace Exthand.FinanceExports
{
    public interface IExportBuilderFactory
    {
        IBaseExportBuilder<T> GetBuilder<T>(BuilderOuputType ouputType);
    }
}
