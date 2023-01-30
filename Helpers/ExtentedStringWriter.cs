using System.IO;
using System.Text;

namespace Exthand.FinanceExports.Helpers
{
    // https://blog.jsinh.in/use-utf-8-encoding-for-stringwriter-in-c/
    public sealed class ExtentedStringWriter : StringWriter
    {
        private readonly Encoding _stringWriterEncoding;

        public ExtentedStringWriter(Encoding desiredEncoding)
        {
            _stringWriterEncoding = desiredEncoding;
        }

        public override Encoding Encoding
        {
            get
            {
                return _stringWriterEncoding;
            }
        }
    }
}
