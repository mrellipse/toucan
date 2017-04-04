using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Toucan.Contract
{
    public interface IExternalAuthenticationService
    {
        Task<Nonce> CreateNonce();
        Task<ClaimsIdentity> RedeemToken(IExternalLogin login);
        Task<bool> ValidateToken(string providerId, string accessToken);
        void RevokeToken(string providerId, string accessToken);
    }

    public struct Nonce
    {
        public readonly DateTime Created;
        public readonly string Hash;
        public Nonce(string hash)
        {
            this.Created = DateTime.Now;
            this.Hash = hash;
        }
    }

    public interface IExternalLogin
    {
        string AccessToken { get; }
        string ExternalId { get; set; }
        string Nonce { get; }
        string ProviderId { get; }
        string Username { get; set; }
    }

    public interface IExternalTokenData
    {
        string aud { get; }
        string email { get; }
        bool email_verified { get; }
        int exp { get; }
        string name { get; }
        string sub { get; }
    }

    public interface IExternalAuthenticationProvider
    {
        string ClientId { get; }
        string ProfileUri { get; }
        string ProviderId { get; }
        string RevokeUri { get; }
        Task<IExternalTokenData> GetProfileDataFromProvider(string accessToken);
        void RevokeToken(string accessToken);
    }
}
