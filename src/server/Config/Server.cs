namespace Toucan.Server
{
    public class Config
    {

        public Config()
        {
        }

        public AntiForgeryConfig AntiForgery { get; set; }
        public string[] Areas { get; set; }
        public string Webroot { get; set; }
        public class AntiForgeryConfig
        {
            public string ClientName { get; set; }
            public string CookieName { get; set; }
            public string HeaderName { get; set; }
            public bool RequireSsl { get; set; }
        }
    }

}
