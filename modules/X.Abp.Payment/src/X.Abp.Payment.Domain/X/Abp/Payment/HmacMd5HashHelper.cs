// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Cryptography;
using System.Text;

namespace X.Abp.Payment;

public static class HmacMd5HashHelper
{
    public static string GetMd5Hash(string hashString, string signature)
    {
        return BitConverter.ToString(new HMACMD5(Encoding.UTF8.GetBytes(signature)).ComputeHash(Encoding.UTF8.GetBytes(hashString))).Replace("-", string.Empty).ToLowerInvariant();
    }
}
