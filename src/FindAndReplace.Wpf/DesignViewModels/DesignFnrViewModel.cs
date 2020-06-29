using System.Collections.ObjectModel;
using FindAndReplace.Wpf.ModelFactories;
using FindAndReplace.Wpf.Models;
using FindAndReplace.Wpf.ViewModels;

namespace FindAndReplace.Wpf.DesignViewModels
{
    public class DesignFnrViewModel : FnrViewModel
    {
        public DesignFnrViewModel()
            : base(null, null, null, null, null, null, null)
        {
            FolderParameters = FolderParametersFactory.GetFolderParameters(100);
            FindParameters = FindParametersFactory.GetFindParameters(100);
            ReplaceParameters = ReplaceParametersFactory.GetReplaceParameters(100);
            ProcessStatus = ProcessStatusFactory.GetProcessStatus(123);
            Results = new ObservableCollection<Result>(ResultFactory.GetResults(5));
            Status = "Sample status here";
        }

    }
}
