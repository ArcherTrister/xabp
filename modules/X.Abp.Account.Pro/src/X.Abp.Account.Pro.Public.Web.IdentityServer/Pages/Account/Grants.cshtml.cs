// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Services;
using IdentityServer4.Stores;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Account.Public.Web.Pages.Account;

namespace X.Abp.Account.Web.Pages.Account;

public class GrantsModel : AccountPageModel
{
    public ICollection<GrantViewModel> Grants { get; } = new List<GrantViewModel>();

    protected IIdentityServerInteractionService Interaction { get; }

    protected IClientStore Clients { get; }

    protected IResourceStore Resources { get; }

    public GrantsModel(IIdentityServerInteractionService interaction,
        IClientStore clients,
        IResourceStore resources)
    {
        Interaction = interaction;
        Clients = clients;
        Resources = resources;
    }

    public virtual async Task OnGetAsync()
    {
        // Grants = new List<GrantViewModel>();
        foreach (var grant in await Interaction.GetAllUserGrantsAsync())
        {
            var client = await Clients.FindClientByIdAsync(grant.ClientId);
            if (client != null)
            {
                var resources = await Resources.FindResourcesByScopeAsync(grant.Scopes);

                var item = new GrantViewModel
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName ?? client.ClientId,
                    ClientLogoUrl = client.LogoUri,
                    ClientUrl = client.ClientUri,
                    Created = grant.CreationTime,
                    Expires = grant.Expiration,
                    IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                    ApiGrantNames = resources.ApiResources.Select(x => x.DisplayName ?? x.Name).ToArray()
                };

                Grants.Add(item);
            }
        }
    }

    public virtual async Task<IActionResult> OnPostRevokeAsync(string clientId)
    {
        await Interaction.RevokeUserConsentAsync(clientId);
        return Redirect("~/");
    }

    public class GrantViewModel
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string ClientUrl { get; set; }

        public string ClientLogoUrl { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Expires { get; set; }

        public IEnumerable<string> IdentityGrantNames { get; set; }

        public IEnumerable<string> ApiGrantNames { get; set; }
    }
}
