using Exthand.FinanceExports.Builders;
using Exthand.FinanceExports.Models;
using System;

namespace Exthand.FinanceExports
{
    public class ExportBuilderFactory : IExportBuilderFactory
    {
        public IBaseExportBuilder<T> GetBuilder<T>(BuilderOuputType outputType)
        {
            if (typeof(T) == typeof(TransactionList))
            {
                switch (outputType)
                {
                    case BuilderOuputType.CAMT:
                        return (IBaseExportBuilder<T>)new CamtBuilder();
                    case BuilderOuputType.MT940:
                        return (IBaseExportBuilder<T>)new Mt940Builder();
                    case BuilderOuputType.CODA:
                        return (IBaseExportBuilder<T>)new CodaBuilder();
                    case BuilderOuputType.CSV:
                        return (IBaseExportBuilder<T>)new CsvBuilder();
                    case BuilderOuputType.HTML:
                    default:
                        return (IBaseExportBuilder<T>)new HtmlBuilder();
                }
            }

            throw new NotImplementedException();
        }
    }
}
