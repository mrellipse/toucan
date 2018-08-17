namespace Toucan.Contract.Security
{
    public static class SecurityClaimValueTypes
    {
        public static string Any = new string(new char[] { Create, Read, Update, Delete });
        public const char Create = 'C';
        public const char Read = 'R';
        public const char Update = 'U';
        public const char Delete = 'D';
        public const char Deny = 'X';
    }
}