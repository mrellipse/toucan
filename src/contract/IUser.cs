
namespace Toucan.Contract
{
    public interface IUser
    {
        string Email { get; }
        bool Enabled { get; set; }
        int UserId { get; }
        string Username { get; }
    }
}