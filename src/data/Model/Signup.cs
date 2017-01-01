using Toucan.Contract;

namespace Toucan.Data.Model
{
    public class Signup : ISignupOptions
    {
        public Signup()
        {
        }

        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool Verified { get; set; }

        public string[] Roles { get; set; }
    }
}
