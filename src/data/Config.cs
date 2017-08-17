namespace Toucan.Data
{
    public class Config
    {
        public Config()
        {
        }
        public const string DbConnectionKey = @"data:connectionString";
        public string ConnectionString { get; set; }
        public string HostKey { get; set; }
        public string HostPattern { get; set; }
    }
}
