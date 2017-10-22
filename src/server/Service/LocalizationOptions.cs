using System.IO;

namespace Toucan.Server.Model
{
    public class LocalizationOptions
    {
        public DirectoryInfo Directory { get; set; }
        public string Pattern { get; set; }
    }
}
