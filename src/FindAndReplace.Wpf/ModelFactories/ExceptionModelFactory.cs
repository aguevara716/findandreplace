using System;
using System.Collections.Generic;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.ModelFactories
{
    public static class ExceptionModelFactory
    {
        public static ExceptionModel GetExceptionModel(int seed)
        {
            try
            {
                throw new NotSupportedException($"Exception number {seed}");
            }
            catch (Exception ex)
            {
                return new ExceptionModel(ex);
            }
        }

        public static IEnumerable<ExceptionModel> GetExceptionModels(int count)
        {
            for (var index = 0; index < count; index++)
                yield return GetExceptionModel(index + 1);
        }

    }
}
