using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Toucan.Contract;

namespace Toucan.Common
{
    public class CryptoHelper : ICryptoService
    {
        public const int MinSaltSizeInBytes = 32 / 8;
        public const int DefaultKeySizeInBytes = 128 / 8;

        public readonly KeyDerivationPrf Prf;

        public CryptoHelper()
        {
            this.Prf = KeyDerivationPrf.HMACSHA1;
        }

        public CryptoHelper(KeyDerivationPrf rng)
        {
            this.Prf = rng;
        }

        public string CreateSalt()
        {
            return this.CreateSalt(MinSaltSizeInBytes);
        }

        public string CreateSalt(int sizeInKb)
        {
            if (sizeInKb < MinSaltSizeInBytes)
                throw new ArgumentOutOfRangeException($"Minimum salt size is {MinSaltSizeInBytes} kb");

            byte[] salt = new byte[sizeInKb * 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public string CreateKey(string salt, string data)
        {
            return CreateKey(salt, data, DefaultKeySizeInBytes);
        }
        
        public string CreateKey(string salt, string data, int keySizeInKb)
        {
            if (salt == null)
                throw new ArgumentNullException($"Argument {nameof(salt)} cannot be null");

            if (Convert.FromBase64String(salt).Length / 8 < MinSaltSizeInBytes)
                throw new ArgumentOutOfRangeException($"Argument {nameof(salt)} is invalid. Minimum salt size is {MinSaltSizeInBytes} kb");

            if (data == null)
                throw new ArgumentNullException($"Argument {nameof(data)} cannot be null");

            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentOutOfRangeException($"Argument {nameof(data)} cannot be empty.");

            var derivedKey = KeyDerivation.Pbkdf2(
                password: data,
                salt: Convert.FromBase64String(salt),
                prf: this.Prf,
                iterationCount: 10000,
                numBytesRequested: keySizeInKb * 8);

            return Convert.ToBase64String(derivedKey);
        }

        public bool CheckKey(string hash, string salt, string data)
        {
            return CreateKey(salt, data) == hash;
        }

        public string CreateFingerprint(string data)
        {
            return RabinFingerPrint.ComputeFingerPrint(data).ToString();
        }
    }
}