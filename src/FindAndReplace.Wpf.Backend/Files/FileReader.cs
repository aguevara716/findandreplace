using System;
using System.IO;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IFileReader
    {
        FileSampleResult GetFileSampleData(string filePath);
        FileSampleResult GetFileSampleData(string filePath, int numberOfBytesToIngest);
    }

    public class FileReader : IFileReader
    {
        private const int DEFAULT_NUMBER_OF_BYTES_TO_INGEST = 10240; // 10 kiB

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
