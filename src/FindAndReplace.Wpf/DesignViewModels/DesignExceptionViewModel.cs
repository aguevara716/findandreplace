using System.Linq;
using FindAndReplace.Wpf.ModelFactories;
using FindAndReplace.Wpf.ViewModels;

namespace FindAndReplace.Wpf.DesignViewModels
{
    public class DesignExceptionViewModel : ExceptionViewModel
    {
        public DesignExceptionViewModel()
            : base(null, null, null)
        {
            Exceptions = ExceptionModelFactory.GetExceptionModels(40).ToList();
        }

    }
}
