namespace Toucan.Contract
{
    public interface ISignupServiceOptions
    {
        string Username { get; }
        bool Enabled { get; }
        string DisplayName { get; }
        string Password { get; }
        bool Verified { get; }
        string[] Roles { get; }
    }
}
