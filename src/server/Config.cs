namespace Toucan.UI
{
    public class Config
    {
        public const string WebrootKey = @"webroot";

        public Config()
        {
        }

        public string Webroot { get; set; }  
        public Toucan.Data.Config Data { get; set; }     
    }
}
