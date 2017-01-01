using System;

namespace Toucan.Contract
{
    public interface ICryptoService
    {
        string CreateSalt();
        string CreateSalt(int sizeInKb);
        string CreateKey(string salt, string data);
        string CreateKey(string salt, string data, int keySizeInKb);
        bool CheckKey(string hash, string salt, string data);
    }
}