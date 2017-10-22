
namespace Toucan.Contract
{
    public interface IUser
    {
        string CultureName { get; }
        string DisplayName { get; }
        string Email { get; }
        string TimeZoneId { get; }
        bool Enabled { get; set; }
        long UserId { get; }
        string Username { get; }
    }
}