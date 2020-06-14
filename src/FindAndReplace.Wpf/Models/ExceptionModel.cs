using System;

namespace FindAndReplace.Wpf.Models
{
    public class ExceptionModel
    {
        public string Type { get; }
        public string Message { get; }
        public string StackTrace { get; }

        public ExceptionModel(Exception ex)
            : this(ex.GetType(), ex.Message, ex.StackTrace)
        {

        }

        public ExceptionModel(Type type, string message, string stackTrace)
            : this(type.Name, message, stackTrace)
        {

        }

        public ExceptionModel(string type, string message, string stackTrace)
        {
            Type = type;
            Message = message;
            StackTrace = stackTrace;
        }

    }
}
