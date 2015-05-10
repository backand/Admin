using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class DuradosException : Exception
    {
        public DuradosException() :
            base()
        {
        }

        public DuradosException(string message) :
            base(message)
        {
        }

        public DuradosException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }

    public class NullFieldException : DuradosException
    {
        public NullFieldException(string fieldName)
            : base("The not nullable field " + fieldName + " has null value.")
        {
        }
    }

    public class MessageException : DuradosException
    {
        public MessageException(string message)
            : base(message)
        {
        }
    }

    public class FileNotFoundException : DuradosException
    {
        public FileNotFoundException(string message)
            : base(message, new System.IO.FileNotFoundException())
        {
        }
    }

    public class DuradosAccessViolationException : DuradosException
    {
        public DuradosAccessViolationException()
            : base()
        {
        }

        public DuradosAccessViolationException(string message)
            : base(message)
        {
        }
    }
    public class DuradosAsyncRunningException : DuradosException
    {
    }

}
