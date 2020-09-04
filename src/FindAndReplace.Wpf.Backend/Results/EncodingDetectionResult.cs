using System.Text;

namespace FindAndReplace.Wpf.Backend.Results
{
    public class EncodingDetectionResult : BaseResult<Encoding>
    {
        public Encoding FileEncoding {  get { return Payload; } }
    }
}
