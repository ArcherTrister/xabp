// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using IdentityServer4.Models;

namespace X.Abp.Account.Web.Pages;

public class ClientInfoModel
{
    public string ClientName { get; set; }

    public string ClientUrl { get; set; }

    public string ClientLogoUrl { get; set; }

    public bool AllowRememberConsent { get; set; }

    public ClientInfoModel(Client client)
    {
        ClientName = client.ClientId;
        ClientUrl = client.ClientUri;
        ClientLogoUrl = client.LogoUri;
        AllowRememberConsent = client.AllowRememberConsent;
    }
}
