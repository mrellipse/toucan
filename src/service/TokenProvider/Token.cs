
namespace Toucan.Service
{
    public struct Token
    {
        public readonly string access_token;
        public readonly int expires_in;
        public Token(string accessToken, int expiresIn)
        {
            this.access_token = accessToken;
            this.expires_in = expiresIn;
        }

    }
}