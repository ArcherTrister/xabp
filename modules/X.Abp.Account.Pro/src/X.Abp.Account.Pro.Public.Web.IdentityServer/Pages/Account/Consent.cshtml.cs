// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace X.Abp.Account.Web.Pages.Account;

public class ConsentModel : AbpPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [BindProperty]
    public ConsentInputModel ConsentInput { get; set; }

    public ClientInfoModel ClientInfo { get; set; }

    protected IIdentityServerInteractionService Interaction { get; }

    protected IClientStore ClientStore { get; }

    protected IResourceStore ResourceStore { get; }

    protected IEventService Events { get; }

    public ConsentModel(
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IResourceStore resourceStore,
        IEventService events)
    {
        Interaction = interaction;
        ClientStore = clientStore;
        ResourceStore = resourceStore;
        Events = events;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var request = await Interaction.GetAuthorizationContextAsync(ReturnUrl);
        if (request == null)
        {
            throw new UserFriendlyException($"No consent request matching request: {ReturnUrl}");
        }

        var client = await ClientStore.FindEnabledClientByIdAsync(request.Client.ClientId);
        if (client == null)
        {
            throw new UserFriendlyException($"Invalid client id: {request.Client.ClientId}");
        }

        var resources = await ResourceStore.FindEnabledResourcesByScopeAsync(request.ValidatedResources.RawScopeValues);
        if (resources == null || (resources.IdentityResources.Count == 0 && resources.ApiResources.Count == 0))
        {
            throw new UserFriendlyException($"No scopes matching: {request.ValidatedResources.RawScopeValues.Aggregate((x, y) => x + ", " + y)}");
        }

        ClientInfo = new ClientInfoModel(client);
        ConsentInput = new ConsentInputModel
        {
            RememberConsent = true,
            IdentityScopes = resources.IdentityResources.Select(x => CreateScopeViewModel(x, true)).ToList(),
        };

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                var scopeVm = CreateScopeViewModel(parsedScope, apiScope, true);
                apiScopes.Add(scopeVm);
            }
        }

        if (resources.OfflineAccess)
        {
            apiScopes.Add(GetOfflineAccessScope(true));
        }

        ConsentInput.ApiScopes = apiScopes;

        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var request = await Interaction.GetAuthorizationContextAsync(ReturnUrl);
        if (request == null)
        {
            throw new UserFriendlyException($"No consent request matching request: {ReturnUrl}");
        }

        if (ConsentInput.UserDecision == "no")
        {
            // emit event
            await Events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
            await Interaction.GrantConsentAsync(request, new ConsentResponse
            {
                Error = AuthorizationError.AccessDenied
            });
            return Redirect("~/");
        }

        if (ConsentInput.IdentityScopes.IsNullOrEmpty() && ConsentInput.ApiScopes.IsNullOrEmpty())
        {
            throw new UserFriendlyException("You must pick at least one permission");
        }

        // emit event
        await Events.RaiseAsync(new ConsentGrantedEvent(
            User.GetSubjectId(),
            request.Client.ClientId,
            request.ValidatedResources.RawScopeValues,
            ConsentInput.GetAllowedScopeNames(),
            ConsentInput.RememberConsent));
        await Interaction.GrantConsentAsync(request, new ConsentResponse
        {
            RememberConsent = ConsentInput.RememberConsent,
            ScopesValuesConsented = ConsentInput.GetAllowedScopeNames()
        });

        return Redirect(ReturnUrl);
    }

    protected virtual ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new ScopeViewModel
        {
            Name = identity.Name,
            DisplayName = identity.DisplayName,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };
    }

    protected virtual ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        var displayName = apiScope.DisplayName ?? apiScope.Name;
        if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
        {
            displayName += ":" + parsedScopeValue.ParsedParameter;
        }

        return new ScopeViewModel
        {
            Name = parsedScopeValue.RawValue,
            DisplayName = displayName,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    protected virtual ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = "Offline Access", // TODO: Localize
            Description = "Access to your applications and resources, even when you are offline",
            Emphasize = true,
            Checked = check
        };
    }
}
