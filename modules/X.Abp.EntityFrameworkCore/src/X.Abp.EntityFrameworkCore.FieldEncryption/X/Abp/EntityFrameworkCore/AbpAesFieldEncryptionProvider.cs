// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Encryption;

using X.EntityFrameworkCore.FieldEncryption;

namespace X.Abp.EntityFrameworkCore;

public class AbpAesFieldEncryptionProvider : IFieldEncryptionProvider, ITransientDependency
{
    protected AbpStringEncryptionOptions Options { get; }

    public AbpAesFieldEncryptionProvider(IOptions<AbpStringEncryptionOptions> options)
    {
        Options = options.Value;
    }

    public string Decrypt(string cipherText)
    {
        if (cipherText.IsNullOrWhiteSpace())
        {
            return null;
        }

        var passPhrase = Options.DefaultPassPhrase;
        var salt = Options.DefaultSalt;
        var cipherTextBytes = Convert.FromBase64String(cipherText);

        using (var password = new Rfc2898DeriveBytes(passPhrase, salt))
        {
            var keyBytes = password.GetBytes(Options.Keysize / 8);
            using (var symmetricKey = Aes.Create())
            {
                symmetricKey.Mode = CipherMode.CBC;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, Options.InitVectorBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var totalReadCount = 0;
                            while (totalReadCount < cipherTextBytes.Length)
                            {
                                var buffer = new byte[cipherTextBytes.Length];
                                var readCount = cryptoStream.Read(buffer, 0, buffer.Length);
                                if (readCount == 0)
                                {
                                    break;
                                }

                                for (var i = 0; i < readCount; i++)
                                {
                                    plainTextBytes[i + totalReadCount] = buffer[i];
                                }

                                totalReadCount += readCount;
                            }

                            return Encoding.UTF8.GetString(plainTextBytes, 0, totalReadCount);
                        }
                    }
                }
            }
        }
    }

    public byte[] Decrypt(byte[] cipherTextBytes)
    {
        if (cipherTextBytes is null || cipherTextBytes.Length == 0)
        {
            return null;
        }

        var passPhrase = Options.DefaultPassPhrase;
        var salt = Options.DefaultSalt;

        using (var password = new Rfc2898DeriveBytes(passPhrase, salt))
        {
            var keyBytes = password.GetBytes(Options.Keysize / 8);
            using (var symmetricKey = Aes.Create())
            {
                symmetricKey.Mode = CipherMode.CBC;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, Options.InitVectorBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using var output = new MemoryStream();
                            cryptoStream.CopyTo(output);
                            return output.ToArray();
                        }
                    }
                }
            }
        }
    }

    public string Encrypt(string plainText)
    {
        if (plainText.IsNullOrWhiteSpace())
        {
            return null;
        }

        var passPhrase = Options.DefaultPassPhrase;
        var salt = Options.DefaultSalt;
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        using (var password = new Rfc2898DeriveBytes(passPhrase, salt))
        {
            var keyBytes = password.GetBytes(Options.Keysize / 8);
            using (var symmetricKey = Aes.Create())
            {
                symmetricKey.Mode = CipherMode.CBC;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, Options.InitVectorBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            var cipherTextBytes = memoryStream.ToArray();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
    }

    public byte[] Encrypt(byte[] plainTextBytes)
    {
        if (plainTextBytes is null || plainTextBytes.Length == 0)
        {
            return null;
        }

        var passPhrase = Options.DefaultPassPhrase;
        var salt = Options.DefaultSalt;

        using (var password = new Rfc2898DeriveBytes(passPhrase, salt))
        {
            var keyBytes = password.GetBytes(Options.Keysize / 8);
            using (var symmetricKey = Aes.Create())
            {
                symmetricKey.Mode = CipherMode.CBC;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, Options.InitVectorBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
        }
    }
}
