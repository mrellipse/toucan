namespace Toucan.Data
{
    public class Config
    {
        public Config()
        {
        }
        public const string DbConnectionKey = @"data:connectionString";
        public string ConnectionString { get; set; }
    }
}
