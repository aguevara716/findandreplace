using System;
using FindAndReplace.Wpf.Models;

namespace FindAndReplace.Wpf.Services
{
    public interface IReplacerMapper
    {
        Replacer Map(FolderParameters folderParameters,
                     FindParameters findParameters,
                     ReplaceParameters replaceParameters);
    }

    public class ReplacerMapper : IReplacerMapper
    {
        public Replacer Map(FolderParameters folderParameters, 
                            FindParameters findParameters, 
                            ReplaceParameters replaceParameters)
        {
            var replacer = new Replacer
            {
                Dir = folderParameters.RootDirectory,
                IncludeSubDirectories = folderParameters.IsRecursive,

                FileMask = folderParameters.FileMask,
                ExcludeFileMask = folderParameters.ExcludeMask,
                ExcludeDir = folderParameters.ExcludeDirectories,
                FindText = findParameters.FindString,
                IsCaseSensitive = findParameters.IsCaseSensitive,
                FindTextHasRegEx = findParameters.IsRegex,
                SkipBinaryFileDetection = findParameters.IsSkippingBinaryDetection,
                IncludeFilesWithoutMatches = findParameters.IsIncludingFilesWithoutMatches,
                ReplaceText = replaceParameters.ReplaceString,
                UseEscapeChars = findParameters.IsUsingEscapeCharacters,
                IsKeepModifiedDate = findParameters.IsRetainingModifiedDate,
            };

            if (!String.IsNullOrEmpty(findParameters.Encoding) && 
                !findParameters.Encoding.StartsWith("Auto Detect", StringComparison.OrdinalIgnoreCase))
                replacer.AlwaysUseEncoding = Utils.GetEncodingByName(findParameters.Encoding);

            return replacer;
        }

    }
}
