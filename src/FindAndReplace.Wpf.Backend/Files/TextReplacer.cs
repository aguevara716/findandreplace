using System;
using System.Text.RegularExpressions;
using FindAndReplace.Wpf.Backend.Parameters;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface ITextReplacer
    {
        TextReplacementResult ReplaceText(TextReplacementParameters textReplacementParameters);
    }

    public class TextReplacer : ITextReplacer
    {
        private TextReplacementResult ValidateParameters(TextReplacementParameters textReplacementParameters)
        {
            if (textReplacementParameters == null)
                return TextReplacementResult.CreateFailure<TextReplacementResult>(string.Empty, "Text replacement parameters are required");
            if (string.IsNullOrEmpty(textReplacementParameters.FileContent))
                return TextReplacementResult.CreateFailure<TextReplacementResult>(string.Empty, "File content is required");
            if (string.IsNullOrEmpty(textReplacementParameters.FilePath))
                return TextReplacementResult.CreateFailure<TextReplacementResult>(textReplacementParameters.FilePath, "File path is required");
            if (string.IsNullOrEmpty(textReplacementParameters.FindText))
                return TextReplacementResult.CreateFailure<TextReplacementResult>(textReplacementParameters.FilePath, "Find text is required");
            if (string.IsNullOrEmpty(textReplacementParameters.ReplaceText))
                return TextReplacementResult.CreateFailure<TextReplacementResult>(textReplacementParameters.FilePath, "Replacement text is required");
            else
                return null;
        }

        public TextReplacementResult ReplaceText(TextReplacementParameters textReplacementParameters)
        {
            var validationErrorResult = ValidateParameters(textReplacementParameters);
            if (validationErrorResult != null)
                return validationErrorResult;

            try
            {
                if (!textReplacementParameters.IsRegexSearch && !textReplacementParameters.IsUsingEscapeCharacters)
                    textReplacementParameters.FindText = Regex.Escape(textReplacementParameters.FindText);

                if (textReplacementParameters.IsUsingEscapeCharacters)
                    textReplacementParameters.ReplaceText = Regex.Unescape(textReplacementParameters.ReplaceText);

                var regexOptions = RegexOptions.Multiline;
                if (!textReplacementParameters.IsCaseSensitive)
                    regexOptions |= RegexOptions.IgnoreCase;

                var updatedContent = Regex.Replace(textReplacementParameters.FileContent,
                                                   textReplacementParameters.FindText,
                                                   textReplacementParameters.ReplaceText,
                                                   regexOptions);
                return TextReplacementResult.CreateSuccess<TextReplacementResult>(textReplacementParameters.FilePath, updatedContent);
            }
            catch (Exception ex)
            {
                return TextReplacementResult.CreateFailure<TextReplacementResult>(textReplacementParameters.FilePath, ex);
            }
        }

    }
}
