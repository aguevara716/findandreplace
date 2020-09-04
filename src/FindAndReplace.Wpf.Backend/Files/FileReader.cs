using System;
using System.IO;
using System.Threading.Tasks;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IFileReader
    {
        FileContentResult GetFileContent(string filePath);
        Task<FileContentResult> GetFileContentAsync(string filePath);

        FileSampleResult GetFileSampleData(string filePath);
        FileSampleResult GetFileSampleData(string filePath, int numberOfBytesToIngest);
    }

    public class FileReader : IFileReader
    {
        private const int DEFAULT_NUMBER_OF_BYTES_TO_INGEST = 10240; // 10 kiB

        public FileContentResult GetFileContent(string filePath)
        {
            return GetFileContentAsync(filePath).Result;
        }

        public async Task<FileContentResult> GetFileContentAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return FileContentResult.CreateFailure<FileContentResult>(filePath, "File path is required");

            try
            {
                var fileContent = string.Empty;
                using (var streamReader = new StreamReader(filePath))
                {
                    fileContent = await streamReader.ReadToEndAsync();
                }
                return FileContentResult.CreateSuccess<FileContentResult>(filePath, fileContent);
            }
            catch (Exception ex)
            {
                return FileContentResult.CreateFailure<FileContentResult>(filePath, "Unable to read contents of file", ex);
            }
        }

        public FileSampleResult GetFileSampleData(string filePath)
        {
            return GetFileSampleData(filePath, DEFAULT_NUMBER_OF_BYTES_TO_INGEST);
        }

        public FileSampleResult GetFileSampleData(string filePath, int numberOfBytesToIngest)
        {
            try
            {
                byte[] fileData;
                using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    var bytesToIngest = Math.Min(fileStream.Length, numberOfBytesToIngest);

                    fileData = new byte[bytesToIngest];

                    fileStream.Read(fileData, 0, (int)bytesToIngest);
                }

                return FileSampleResult.CreateSuccess<FileSampleResult>(filePath, fileData);
            }
            catch (Exception ex)
            {
                return FileSampleResult.CreateFailure<FileSampleResult>(filePath, "Unable to read file", ex);
            }
        }

    }
}
