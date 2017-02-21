namespace Toucan.Server
{
    public class Config
    {
        public Config()
        {
        }

        public string Webroot { get; set; }
        public string[] Areas { get; set; }
        public Toucan.Data.Config Data { get; set; }
        public Toucan.Service.Config Service { get; set; }
    }
}
