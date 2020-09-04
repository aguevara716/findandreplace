using System;
using System.Linq;
using System.Text;
using FindAndReplace.Wpf.Backend.Extensions;
using FindAndReplace.Wpf.Backend.Results;

namespace FindAndReplace.Wpf.Backend.Files
{
    public interface IEncodingDetector
    {
        EncodingDetectionResult DetectFileEncoding(string filePath, byte[] sampleBytes);
        EncodingDetectionResult DetectFileEncoding(string filePath, byte[] sampleBytes, Encoding fallbackEncoding);
    }

    public class EncodingDetector : IEncodingDetector
    {
        private readonly Encoding FALLBACK_ENCODING = null;
        private const FindAndReplace.EncodingDetector.Options DEFAULT_OPTIONS = FindAndReplace.EncodingDetector.Options.KlerkSoftBom | FindAndReplace.EncodingDetector.Options.MLang;

        public EncodingDetectionResult DetectFileEncoding(string filePath, byte[] sampleBytes)
        {
            return DetectFileEncoding(filePath, sampleBytes, FALLBACK_ENCODING);
        }

        public EncodingDetectionResult DetectFileEncoding(string filePath, byte[] sampleBytes, Encoding fallbackEncoding)
        {
            if (string.IsNullOrEmpty(filePath))
                return EncodingDetectionResult.CreateFailure<EncodingDetectionResult>(filePath, "File path was not specified");
            if (sampleBytes.IsNullOrEmpty())
                return EncodingDetectionResult.CreateFailure<EncodingDetectionResult>(filePath, "File sample was not specified");

            try
            {
                var detectedEncoding = DetectEncoding(sampleBytes, DEFAULT_OPTIONS, fallbackEncoding);
                if (detectedEncoding == null)
                    return EncodingDetectionResult.CreateFailure<EncodingDetectionResult>(filePath, $"Unable to detect encoding for file \"{filePath}\"");

                return EncodingDetectionResult.CreateSuccess<EncodingDetectionResult>(filePath, detectedEncoding);
            }
            catch (Exception ex)
            {
                return EncodingDetectionResult.CreateFailure<EncodingDetectionResult>(filePath, $"Failed to detect encoding for file \"{filePath}\"", ex);
            }
        }

        private Encoding DetectEncoding(byte[] sampleBytes, FindAndReplace.EncodingDetector.Options options, Encoding fallbackEncoding)
        {
            Encoding detectedEncoding = null;
            if ((options & FindAndReplace.EncodingDetector.Options.KlerkSoftBom) == FindAndReplace.EncodingDetector.Options.KlerkSoftBom)
                detectedEncoding = DetectEncodingUsinglerksSoftBom(sampleBytes);
            if (detectedEncoding != null)
                return detectedEncoding;

            if ((options & FindAndReplace.EncodingDetector.Options.KlerkSoftHeuristics) == FindAndReplace.EncodingDetector.Options.KlerkSoftHeuristics)
                detectedEncoding = DetectEncodingUsingKlerksSoftHeuristics(sampleBytes);
            if (detectedEncoding != null)
                return detectedEncoding;

            if ((options & FindAndReplace.EncodingDetector.Options.MLang) == FindAndReplace.EncodingDetector.Options.MLang)
                detectedEncoding = DetectEncodingUsingMlang(sampleBytes);
            if (detectedEncoding != null)
                return detectedEncoding;

            return fallbackEncoding;
        }

        private Encoding DetectEncodingUsinglerksSoftBom(byte[] sampleBytes)
        {
            if (sampleBytes.Length < 4)
                return null;

            return KlerksSoftEncodingDetector.DetectBOMBytes(sampleBytes);
        }

        private Encoding DetectEncodingUsingKlerksSoftHeuristics(byte[] sampleBytes)
        {
            return KlerksSoftEncodingDetector.DetectUnicodeInByteSampleByHeuristics(sampleBytes);
        }

        private Encoding DetectEncodingUsingMlang(byte[] sampleBytes)
        {
            try
            {
                var detectedEncodings = EncodingTools.EncodingTools.DetectInputCodepages(sampleBytes, 5);
                if (!detectedEncodings.Any())
                    return null;

                var firstEncoding = detectedEncodings.Last();
                return firstEncoding;
            }
            catch
            {
                return null;
            }
        }

    }
}
