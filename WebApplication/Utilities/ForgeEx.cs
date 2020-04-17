using System;
using System.Text;
using Autodesk.Forge.Core;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Extensions for Forge stuff.
    /// </summary>
    public static class ForgeEx
    {
        /// <summary>
        /// Ensure the configuration is valid.
        /// </summary>
        public static ForgeConfiguration Validate(this ForgeConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.ClientId)) throw new ArgumentException("Forge Client ID is not provided.");
            if (string.IsNullOrEmpty(configuration.ClientSecret)) throw new ArgumentException("Forge Client Secret is not provided.");

            return configuration;
        }

        public static byte[] Hash(this ForgeConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-3.1
            return KeyDerivation.Pbkdf2(
                                        password: configuration.ClientId,
                                        salt: Encoding.UTF8.GetBytes(configuration.ClientSecret + configuration.ClientId), // ER: is it OK? Or safer to not use the secret at all?
                                        prf: KeyDerivationPrf.HMACSHA1,
                                        iterationCount: 10,
                                        numBytesRequested: 12);
        }

        public static string HashString(this ForgeConfiguration configuration)
        {
            var hashBytes = configuration.Hash();
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
