
namespace Toucan.Contract
{
    public interface IUser
    {
        string DisplayName { get; }
        string Email { get; }
        bool Enabled { get; set; }
        long UserId { get; }
        string Username { get; }
    }
}