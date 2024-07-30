// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.IdentityServer.Client.Dtos;

public class CreateClientDto : ExtensibleObject
{
    [Required]
    [DynamicStringLength(typeof(ClientConsts), "ClientIdMaxLength", null)]
    public string ClientId { get; set; }

    [Required]
    [DynamicStringLength(typeof(ClientConsts), "ClientNameMaxLength", null)]
    public string ClientName { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "DescriptionMaxLength", null)]
    public string Description { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "ClientUriMaxLength", null)]
    public string ClientUri { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "LogoUriMaxLength", null)]
    public string LogoUri { get; set; }

    public bool RequireConsent { get; set; }

    public string CallbackUrl { get; set; }

    public string LogoutUrl { get; set; }

    public ClientSecretDto[] Secrets { get; set; }

    public string[] Scopes { get; set; }
}
