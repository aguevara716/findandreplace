using System;
using System.Text;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Services
{
    public interface IExceptionFormatter
    {
        string FormatException(params ExceptionModel[] exceptions);
    }

    public class ExceptionFormatter : IExceptionFormatter
    {
        private const string SEPARATOR = "==========";

        public string FormatException(params ExceptionModel[] exceptions)
        {
            if (exceptions == null || exceptions.Length == 0)
                return String.Empty;

            var exceptionBuilder = new StringBuilder();
            exceptionBuilder.AppendLine($"Exceptions caught: {exceptions.Length:N0}");
            for (var index = 0; index < exceptions.Length; index++)
            {
                var exception = exceptions[index];
                exceptionBuilder.AppendLine($"{index + 1} - Caught: {exception.Type}");
                exceptionBuilder.AppendLine($"Message: {exception.Message}");
                exceptionBuilder.AppendLine($"Stack Trace:\n{exception.StackTrace}");
                exceptionBuilder.AppendLine(SEPARATOR);
            }
            return exceptionBuilder.ToString();
        }

    }

}
