using System;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Services
{
    public interface IFinderMapper
    {
        Finder Map(FolderParameters folderParameters, FindParameters findParameters);
    }

    public class FinderMapper : IFinderMapper
    {
        public Finder Map(FolderParameters folderParameters,
                          FindParameters findParameters)
        {
            var finder = new Finder
            {
                Dir = folderParameters.RootDirectory,
                IncludeSubDirectories = folderParameters.IsRecursive,
                FileMask = folderParameters.IncludeFilesString,
                ExcludeFileMask = folderParameters.ExcludeFilesString,
                ExcludeDir = folderParameters.ExcludeDirectoriesString,

                FindText = findParameters.FindString,
                FindTextHasRegEx = findParameters.IsRegex,
                IsCaseSensitive = findParameters.IsCaseSensitive,
                SkipBinaryFileDetection = findParameters.IsSkippingBinaryDetection,
                IncludeFilesWithoutMatches = findParameters.IsIncludingFilesWithoutMatches,
                UseEscapeChars = findParameters.IsUsingEscapeCharacters
            };

            if(!String.IsNullOrEmpty(findParameters.Encoding) &&
                !findParameters.Encoding.StartsWith("Auto Detect", StringComparison.OrdinalIgnoreCase))
                finder.AlwaysUseEncoding = Utils.GetEncodingByName(findParameters.Encoding);

            return finder;
        }
    }
}
