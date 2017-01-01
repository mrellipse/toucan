using System;
using Xunit;
using Toucan.Common;

namespace Toucan.CommonTests
{
    public class CryptoHelperTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        public void MinSaltLengthIsEnforced(int size)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CryptoHelper().CreateSalt(size));
        }

        [Fact]
        public void MinSaltLengthIsUsedWhenSizeArgumentNotProvider()
        {
            var salt = new CryptoHelper().CreateSalt();
            byte[] bytes = Convert.FromBase64String(salt);
            Assert.Equal(CryptoHelper.MinSaltSizeInBytes * 8, bytes.Length);
        }

        [Theory]
        [InlineData(16)]
        [InlineData(24)]
        [InlineData(32)]
        public void CreateSaltOfSpecifiedLength(int size)
        {
            var salt = new CryptoHelper().CreateSalt(size);
            byte[] bytes = Convert.FromBase64String(salt);
            Assert.Equal(size * 8, bytes.Length);
        }

        [Fact]
        public void CreateKeyArgumentGuards()
        {
            var helper = new CryptoHelper();

            Assert.Throws<ArgumentNullException>(() =>
            {
                helper.CreateKey(null, null);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                helper.CreateKey("", null);
            });

            var salt = helper.CreateSalt();

            Assert.Throws<ArgumentNullException>(() =>
            {
                helper.CreateKey(salt, null);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                helper.CreateKey(salt, string.Empty);
            });
        }

        [Fact]
        public void DefaultKeyLengthIsUsedWhenNoneIsProvided()
        {
            var helper = new CryptoHelper();
            var hash = helper.CreateKey(helper.CreateSalt(), "123");
            byte[] bytes = Convert.FromBase64String(hash);
            Assert.Equal(CryptoHelper.DefaultKeySizeInBytes * 8, bytes.Length);
        }

        [Fact]
        public void CheckKeyPass()
        {
            var helper = new CryptoHelper();
            var salt = helper.CreateSalt();
            var hash = helper.CreateKey(salt, "letmein");

            Assert.True(helper.CheckKey(hash, salt, "letmein"));
        }

        [Fact]
        public void CheckKeyFail()
        {
            var helper = new CryptoHelper();
            var salt = helper.CreateSalt();
            var hash = helper.CreateKey(salt, "letmein");

            Assert.False(helper.CheckKey(hash, salt, "letmeout"));
        }
    }
}
