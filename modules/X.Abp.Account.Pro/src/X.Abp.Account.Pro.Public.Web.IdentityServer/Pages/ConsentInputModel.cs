// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;

namespace X.Abp.Account.Web.Pages;

public class ConsentInputModel
{
    public List<ScopeViewModel> IdentityScopes { get; set; }

    public List<ScopeViewModel> ApiScopes { get; set; }

    public string UserDecision { get; set; }

    public bool RememberConsent { get; set; }

    public ICollection<string> GetAllowedScopeNames()
    {
        var identityScopes = IdentityScopes ?? new List<ScopeViewModel>();
        var apiScopes = ApiScopes ?? new List<ScopeViewModel>();
        return identityScopes.Union(apiScopes).Where(s => s.Checked || s.Required).Select(s => s.Name).ToList();
    }
}
