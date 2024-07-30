// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using CertificateManager;
using CertificateManager.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace X.Abp.Cli.CertGenerating;

public class CertGenerator : ICertGenerator
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    protected ILogger<CertGenerator> Logger { get; set; }

    public CertGenerator(IServiceScopeFactory serviceScopeFactory, ILogger<CertGenerator> logger)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Logger = logger ?? NullLogger<CertGenerator>.Instance;
    }

    public async Task GenerateCertAsync(CertGenerateArgs args)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var cc = scope.ServiceProvider.GetRequiredService<CreateCertificates>();
        if (cc == null)
        {
            throw new ArgumentNullException($"{typeof(CreateCertificates)} is null");
        }

        var iec = scope.ServiceProvider.GetRequiredService<ImportExportCertificate>();
        if (iec == null)
        {
            throw new ArgumentNullException($"{typeof(ImportExportCertificate)} is null");
        }

        if (args.CertType.Equals("rsa", StringComparison.OrdinalIgnoreCase))
        {
            using var rsaCert = CreateRsaCertificate(cc, args.DnsName, args.ValidityPeriodInYears);
            var rsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(args.Password, rsaCert);
            await File.WriteAllBytesAsync(Path.Combine(args.OutputFolderRoot, $"{args.CertName}.pfx"), rsaCertPfxBytes);
            Logger.LogInformation("rsa cert file generate success!");
        }
        else
        {
            using var rsaCert = CreateECDsaCertificate(cc, args.DnsName, args.ValidityPeriodInYears);
            var rsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(args.Password, rsaCert);
            await File.WriteAllBytesAsync(Path.Combine(args.OutputFolderRoot, $"{args.CertName}.pfx"), rsaCertPfxBytes);
            Logger.LogInformation("ecd cert file generate success!");
        }
    }

    private static X509Certificate2 CreateRsaCertificate(CreateCertificates cc, string dnsName, int validityPeriodInYears)
    {
        var basicConstraints = new BasicConstraints
        {
            CertificateAuthority = false,
            HasPathLengthConstraint = false,
            PathLengthConstraint = 0,
            Critical = false
        };

        var subjectAlternativeName = new SubjectAlternativeName
        {
            DnsName = new List<string> { dnsName }
        };

        var x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature;

        // only if certification authentication is used
        var enhancedKeyUsages = new OidCollection
        {
            new Oid("1.3.6.1.5.5.7.3.1"),  // TLS Server auth
            new Oid("1.3.6.1.5.5.7.3.2"),  // TLS Client auth
        };

        var certificate = cc.NewRsaSelfSignedCertificate(
            new DistinguishedName { CommonName = dnsName },
            basicConstraints,
            new ValidityPeriod
            {
                ValidFrom = DateTimeOffset.UtcNow,
                ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears)
            },
            subjectAlternativeName,
            enhancedKeyUsages,
            x509KeyUsageFlags,
            new RsaConfiguration { KeySize = 2048 });

        return certificate;
    }

    private static X509Certificate2 CreateECDsaCertificate(CreateCertificates cc, string dnsName, int validityPeriodInYears)
    {
        var basicConstraints = new BasicConstraints
        {
            CertificateAuthority = false,
            HasPathLengthConstraint = false,
            PathLengthConstraint = 0,
            Critical = false
        };

        var san = new SubjectAlternativeName
        {
            DnsName = new List<string> { dnsName }
        };

        var x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature;

        // only if certification authentication is used
        var enhancedKeyUsages = new OidCollection
        {
            new Oid("1.3.6.1.5.5.7.3.1"),  // TLS Server auth
            new Oid("1.3.6.1.5.5.7.3.2"),  // TLS Client auth
        };

        var certificate = cc.NewECDsaSelfSignedCertificate(
            new DistinguishedName { CommonName = dnsName },
            basicConstraints,
            new ValidityPeriod
            {
                ValidFrom = DateTimeOffset.UtcNow,
                ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears)
            },
            san,
            enhancedKeyUsages,
            x509KeyUsageFlags,
            new ECDsaConfiguration());

        return certificate;
    }
}
