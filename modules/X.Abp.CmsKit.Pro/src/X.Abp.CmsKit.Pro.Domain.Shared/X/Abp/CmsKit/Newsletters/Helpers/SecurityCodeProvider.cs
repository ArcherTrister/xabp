// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Volo.Abp.DependencyInjection;

namespace X.Abp.CmsKit.Newsletters.Helpers;

public class SecurityCodeProvider : ITransientDependency
{
    public static string Salt { private get; set; } = "this-is-a-abp-newsletter-key";

    public virtual string GetSecurityCode(string x)
    {
        using var hmacshA1 = new HMACSHA1(Encoding.UTF8.GetBytes(Salt));
        return string.Concat(hmacshA1.ComputeHash(Encoding.UTF8.GetBytes(x)).Select(cmsKitPro => cmsKitPro.ToString("x2")));
    }
}
