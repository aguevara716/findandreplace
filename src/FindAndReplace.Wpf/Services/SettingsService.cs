using FindAndReplace.Wpf.Models;
using FindAndReplace.Wpf.Properties;

namespace FindAndReplace.Wpf.Services
{
    public interface ISettingsService
    {
        void SaveSettings(FolderParameters folderParameters, FindParameters findParameters, ReplaceParameters replaceParameters);
        SettingsTuple LoadSettings();
    }

    public class SettingsService : ISettingsService
    {
        public SettingsTuple LoadSettings()
        {
            Settings.Default.Upgrade();
            Settings.Default.Save();

            var folderParameters = new FolderParameters
            {
                ExcludeDirectories = Settings.Default.ExcludeDirectories,
                ExcludeMask = Settings.Default.ExcludeMask,
                FileMask = Settings.Default.FileMask,
                IsRecursive = Settings.Default.IsRecursive,
                RootDirectory = Settings.Default.RootDirectory,
            };
            var findParameters = new FindParameters
            {
                Encoding = Settings.Default.Encoding,
                FindString = Settings.Default.FindString,
                IsCaseSensitive = Settings.Default.IsCaseSensitive,
                IsIncludingFilesWithoutMatches = Settings.Default.IsIncludingFilesWithoutMatches,
                IsRegex = Settings.Default.IsRegex,
                IsRetainingModifiedDate = Settings.Default.IsRetainingModifiedDate,
                IsShowingEncoding = Settings.Default.IsShowingEncoding,
                IsSkippingBinaryDetection = Settings.Default.IsSkippingBinaryDetection,
                IsUsingEscapeCharacters = Settings.Default.IsUsingEscapeCharacters
            };
            var replaceParameters = new ReplaceParameters
            {
                ReplaceString = Settings.Default.ReplaceString
            };

            var settingsTuple = new SettingsTuple(folderParameters, findParameters, replaceParameters);
            return settingsTuple;
        }

        public void SaveSettings(FolderParameters folderParameters, FindParameters findParameters, ReplaceParameters replaceParameters)
        {
            Settings.Default.ExcludeDirectories = folderParameters.ExcludeDirectories;
            Settings.Default.ExcludeMask = folderParameters.ExcludeMask;
            Settings.Default.FileMask = folderParameters.FileMask;
            Settings.Default.IsRecursive = folderParameters.IsRecursive;
            Settings.Default.RootDirectory = folderParameters.RootDirectory;

            Settings.Default.Encoding = findParameters.Encoding;
            Settings.Default.FindString = findParameters.FindString;
            Settings.Default.IsCaseSensitive = findParameters.IsCaseSensitive;
            Settings.Default.IsIncludingFilesWithoutMatches = findParameters.IsIncludingFilesWithoutMatches;
            Settings.Default.IsRegex = findParameters.IsRegex;
            Settings.Default.IsRetainingModifiedDate = findParameters.IsRetainingModifiedDate;
            Settings.Default.IsShowingEncoding = findParameters.IsShowingEncoding;
            Settings.Default.IsSkippingBinaryDetection = findParameters.IsSkippingBinaryDetection;
            Settings.Default.IsUsingEscapeCharacters = findParameters.IsUsingEscapeCharacters;

            Settings.Default.ReplaceString = replaceParameters.ReplaceString;

            Settings.Default.Save();
        }
    }
}
