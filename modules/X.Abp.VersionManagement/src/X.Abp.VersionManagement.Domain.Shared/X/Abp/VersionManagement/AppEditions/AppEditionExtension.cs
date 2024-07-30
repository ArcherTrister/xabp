// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace X.Abp.VersionManagement.AppEditions;

public static class AppEditionExtension
{
    public static string GetUniqueFileDownloadName(string appName, PlatformType platformType, string arch, string channel, string version, string fileExt)
    {
        return platformType == PlatformType.Mac ?
            $"{appName}-Install-{version}-{arch}-{channel}{fileExt}" :
            platformType == PlatformType.Windows ?
            $"{appName}-Setup-{version}-{arch}-{channel}{fileExt}" :
            $"{appName}-{version}-{arch}-{channel}{fileExt}";
    }

    public static PlatformType GetPlatformType(string platform)
    {
        try
        {
            return (PlatformType)Enum.Parse(typeof(PlatformType), platform);
        }
        catch (Exception)
        {
            return platform switch
            {
                // case 'aix':
                //      console.log("IBM AIX platform");
                //      break;
                //  case 'freebsd':
                //      console.log("FreeBSD Platform");
                //      break;
                //  case 'linux':
                //      console.log("Linux Platform");
                //      break;
                //  case 'openbsd':
                //      console.log("OpenBSD platform");
                //      break;
                //  case 'sunos':
                //      console.log("SunOS platform");
                //      break;
                "win32" => PlatformType.Windows,
                "darwin" => PlatformType.Mac,
                "android" => PlatformType.Android,
                "linux" => PlatformType.Linux,
                _ => throw new ArgumentException("未知的平台类型!"),
            };
        }
    }

    /// <summary>
    /// ToHexString
    /// </summary>
    /// <param name="plainString">plainString</param>
    /// <param name="encode">encode</param>
    public static string ToHexString(this string plainString, Encoding encode)
    {
        return BitConverter.ToString(encode.GetBytes(plainString)).Replace("-", " ");
    }

    public static string ToHexString(byte[] value)
    {
        return BitConverter.ToString(value).Replace("-", " ");
    }

    public static string GetHash(byte[] bytes, string type = "sha1", string convertType = "hex")
    {
        if (type.Equals("sha1", StringComparison.OrdinalIgnoreCase))
        {
            using var mySHA1 = SHA1.Create();
            var hashValue = mySHA1.ComputeHash(bytes);
            return convertType.Equals("hex", StringComparison.OrdinalIgnoreCase) ? ToHexString(hashValue) : Convert.ToBase64String(hashValue);
        }
        else if (type.Equals("sha256", StringComparison.OrdinalIgnoreCase))
        {
            using var mySHA256 = SHA256.Create();
            var hashValue = mySHA256.ComputeHash(bytes);
            return convertType.Equals("hex", StringComparison.OrdinalIgnoreCase) ? ToHexString(hashValue) : Convert.ToBase64String(hashValue);
        }
        else
        {
            using var mySHA512 = SHA512.Create();
            var hashValue = mySHA512.ComputeHash(bytes);
            return convertType.Equals("hex", StringComparison.OrdinalIgnoreCase) ? ToHexString(hashValue) : Convert.ToBase64String(hashValue);
        }
    }

    public static byte[] ToBytes(this Stream stream)
    {
        var bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);

        // 设置当前流的位置为流的开始
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }
}
