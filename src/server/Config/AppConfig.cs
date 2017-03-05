namespace Toucan.Server
{
    public class AppConfig
    {
        public AppConfig()
        {
        }

        public Toucan.Data.Config Data { get; set; }
        public Toucan.Server.Config Server { get; set; }
        public Toucan.Service.Config Service { get; set; }
    }
}
