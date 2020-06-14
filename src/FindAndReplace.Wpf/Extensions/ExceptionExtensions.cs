using System;
using System.Collections.Generic;

namespace FindAndReplace.Wpf.Extensions
{
    public static class ExceptionExtensions
    {
        public static List<Exception> Flatten(this Exception exception)
        {
            var exList = new List<Exception>();
            var currentException = exception;
            do
            {
                exList.Add(currentException);
                currentException = currentException.InnerException;
            } while (currentException.InnerException != null);

            return exList;
        }

    }
}
