using System;
using FindAndReplace.Wpf.Models;
using FindAndReplace.Wpf.ViewModels;

namespace FindAndReplace.Wpf.DesignViewModels
{
    public class DesignExceptionViewModel : ExceptionViewModel
    {
        public DesignExceptionViewModel()
            : base(null, null, null)
        {
            for(var index = 0; index < 40; index++)
            {
                var em = GenerateException(index + 1);
                Exceptions.Add(em);
            }
        }

        private ExceptionModel GenerateException(int index)
        {
            try
            {
                throw new Exception($"Exception number {index}");
            }
            catch (Exception ex)
            {
                return new ExceptionModel(ex);
            }
        }

    }
}
