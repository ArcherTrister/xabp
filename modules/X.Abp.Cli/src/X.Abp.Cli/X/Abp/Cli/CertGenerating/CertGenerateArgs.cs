// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Cli.CertGenerating;

public class CertGenerateArgs
{
    /// <summary>
    /// 输出文件夹
    /// </summary>
    public string OutputFolderRoot { get; }

    /// <summary>
    /// 证书名称
    /// </summary>
    public string CertName { get; }

    /// <summary>
    /// 证书类型 RSA ECD
    /// </summary>
    public string CertType { get; }

    /// <summary>
    /// 证书密码
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Dns Name
    /// </summary>
    public string DnsName { get; set; }

    /// <summary>
    /// 证书年限
    /// </summary>
    public int ValidityPeriodInYears { get; }

    public CertGenerateArgs(
        string outputFolderRoot,
        string certName,
        string certType,
        string password,
        string dnsName,
        int validityPeriodInYears)
    {
        OutputFolderRoot = outputFolderRoot;
        CertName = certName;
        CertType = certType;
        Password = password;
        DnsName = dnsName;
        ValidityPeriodInYears = validityPeriodInYears;
    }
}
