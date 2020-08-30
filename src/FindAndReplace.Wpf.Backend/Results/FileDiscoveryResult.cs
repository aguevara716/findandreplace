using System;
using System.Collections.Generic;

namespace FindAndReplace.Wpf.Backend.Results
{
    public class FileDiscoveryResult : BaseResult<IList<String>>
    {
        public IList<String> Files { get { return Payload; } }
    }
}
