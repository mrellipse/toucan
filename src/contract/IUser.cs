
namespace Toucan.Contract
{
    public interface IUser
    {
        string Email { get; }
        bool Enabled { get; set; }
        long UserId { get; }
        string Username { get; }
    }
}