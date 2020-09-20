using System;
using System.IO;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Parameters;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IFileWriter
    {
        Task<FileWriterResult> WriteTextAsync(FileWriterParameters fileWriterParameters);
    }

    public class FileWriter : IFileWriter
    {
        private FileWriterResult ValidateParameters(FileWriterParameters fileWriterParameters)
        {
            if (fileWriterParameters == null)
                return FileWriterResult.CreateFailure<FileWriterResult>(string.Empty, "File writer parameters are required");
            if (string.IsNullOrEmpty(fileWriterParameters.FilePath))
                return FileWriterResult.CreateFailure<FileWriterResult>(fileWriterParameters.FilePath, "File path is required");
            if (fileWriterParameters.NewFileContent == null)
                return FileWriterResult.CreateFailure<FileWriterResult>(fileWriterParameters.FilePath, "Content is required");
            else
                return null;
        }

        public async Task<FileWriterResult> WriteTextAsync(FileWriterParameters fileWriterParameters)
        {
            var validationErrorResult = ValidateParameters(fileWriterParameters);
            if (validationErrorResult != null)
                return validationErrorResult;

            try
            {
                var modifiedDate = fileWriterParameters.IsKeepingOriginalModificationDate
                    ? DateTime.Now
                    : File.GetLastWriteTime(fileWriterParameters.FilePath);

                using (var streamWriter = new StreamWriter(fileWriterParameters.FilePath, false))
                {
                    await streamWriter.WriteAsync(fileWriterParameters.NewFileContent);
                }

                File.SetLastWriteTime(fileWriterParameters.FilePath, modifiedDate);

                return FileWriterResult.CreateSuccess<FileWriterResult>(fileWriterParameters.FilePath, true);
            }
            catch (Exception ex)
            {
                return FileWriterResult.CreateFailure<FileWriterResult>(fileWriterParameters.FilePath, ex);
            }
        }

    }
}
