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

    public class AppNotFoundException : DuradosException
    {
        public AppNotFoundException()
            : base("App was not found.")
        {
        }

        public AppNotFoundException(string appName)
            : base(String.Format("The app \"{0}\" was not found", appName))
        {
        }
    }

    public class UserNotFoundException : DuradosException
    {
        public UserNotFoundException()
            : base("User was not found.")
        {
        }

        public UserNotFoundException(string username)
            : base(String.Format("The user \"{0}\" was not found", username))
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

    public class DuplicateFieldException : DuradosException
    {
        public DuplicateFieldException(string fieldName)
            : base(String.Format("The field {0} already exists", fieldName))
        {
             
        }

        public DuplicateFieldException(string fieldName, string tableName, string appName)
            : base(String.Format("The field {0} already exists in object {1}, in app {2}", fieldName, tableName, appName))
        {

        }
    }

}
