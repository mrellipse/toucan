using Toucan.Contract;

namespace Toucan.Service.Model
{
    public class LocalSignupOptions : ILocalSignupOptions
    {
        public string Username { get; set; }
        public bool Enabled { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public bool Verified { get; set; }
        public string[] Roles { get; set; }
    }
}