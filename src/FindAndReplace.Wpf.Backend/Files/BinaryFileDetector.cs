using System;
using System.Text;
using FindAndReplace.Wpf.Backend.Extensions;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IBinaryFileDetector
    {
        BinaryFileDetectionResult CheckIsBinaryFile(string filePath, byte[] sampleBytes);
    }

    public class BinaryFileDetector : IBinaryFileDetector
    {
        private const string BINARY_FILE_CONTENT = "\0\0\0\0";

        public BinaryFileDetectionResult CheckIsBinaryFile(string filePath, byte[] sampleBytes)
        {
            if (string.IsNullOrEmpty(filePath))
                return BinaryFileDetectionResult.CreateFailure<BinaryFileDetectionResult>(filePath, "File path is required");
            if (sampleBytes.IsNullOrEmpty())
                return BinaryFileDetectionResult.CreateFailure<BinaryFileDetectionResult>(filePath, "Sample data was not provided");
            try
            {
                var fileText = Encoding.Default.GetString(sampleBytes);

                var isBinaryFile = fileText.Contains(BINARY_FILE_CONTENT);
                return BinaryFileDetectionResult.CreateSuccess<BinaryFileDetectionResult>(filePath, isBinaryFile);
            }
            catch (Exception ex)
            {
                return BinaryFileDetectionResult.CreateFailure<BinaryFileDetectionResult>(filePath, $"Unable to detect whether \"{filePath}\" is a binary file", ex);
            }
        }

    }
}
