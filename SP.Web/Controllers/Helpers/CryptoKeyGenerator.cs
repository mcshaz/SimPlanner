using System;
using System.Security.Cryptography;

namespace SP.Web.Controllers.Helpers
{
    public static class CryptoKeyGenerator
    {
        public static string Create(int bits = 128)
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[bits];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }
    }
}