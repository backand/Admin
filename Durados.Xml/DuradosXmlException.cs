using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Xml
{
    public class DuradosXmlException : Exception
    {
        public DuradosXmlException() :
            base()
        {
        }

        public DuradosXmlException(string message) :
            base(message)
        {
        }

        public DuradosXmlException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }



    public class FileNotFoundException : DuradosXmlException
    {
        public FileNotFoundException(string message)
            : base(message, new System.IO.FileNotFoundException())
        {
        }
    }
}
