namespace FindAndReplace.Wpf.Models
{
    public class SettingsTuple
    {
        public FolderParameters FolderParameters { get; }
        public FindParameters FindParameters { get; }
        public ReplaceParameters ReplaceParameters { get; }

        public SettingsTuple(FolderParameters folderParameters, 
                             FindParameters findParameters, 
                             ReplaceParameters replaceParameters)
        {
            FolderParameters = folderParameters;
            FindParameters = findParameters;
            ReplaceParameters = replaceParameters;
        }

    }
}
