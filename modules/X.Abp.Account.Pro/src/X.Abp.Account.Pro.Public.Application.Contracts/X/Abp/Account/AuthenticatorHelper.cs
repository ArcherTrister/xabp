using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace X.Abp.Account;
public static class AuthenticatorHelper
{
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    public static string FormatKey(string unformattedKey)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int i;
        for (i = 0; i + 4 < unformattedKey.Length; i += 4)
        {
            stringBuilder.Append(unformattedKey.Substring(i, 4)).Append(' ');
        }

        if (i < unformattedKey.Length)
        {
            stringBuilder.Append(unformattedKey.Substring(i));
        }

        return stringBuilder.ToString().ToLowerInvariant();
    }

    public static string GenerateQrCodeUri(string email, string unformattedKey, string applicationName)
    {
        return string.Format(CultureInfo.InvariantCulture, "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6", UrlEncoder.Default.Encode(applicationName), UrlEncoder.Default.Encode(email), unformattedKey);
    }
}
