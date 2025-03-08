using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace DbcLib.Helper
{
    internal class ParserHelper : IDisposable
    {
        internal int count;
        private StreamReader streamReader;
        private StringBuilder stringBuilder;
        public string Error { get { return stringBuilder.ToString(); } }
        public ParserHelper(string path, Encoding encoding)
        {
            stringBuilder = new StringBuilder();
            streamReader = new StreamReader(path, encoding);
        }
        public void Dispose()
        {
            streamReader?.Dispose();
        }
        internal string GetLine()
        {
            count++;
            return streamReader?.ReadLine();
        }
        internal void Exception(string exception)
        {
#if DEBUG
            throw new Exception(exception);
#else
            stringBuilder?.AppendLine($"error {count}:" + exception);
#endif
        }
    }
}