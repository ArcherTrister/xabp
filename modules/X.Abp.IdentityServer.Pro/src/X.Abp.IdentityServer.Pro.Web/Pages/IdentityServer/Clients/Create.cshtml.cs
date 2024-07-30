// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using X.Abp.IdentityServer.ApiResource;
using X.Abp.IdentityServer.ApiResource.Dtos;
using X.Abp.IdentityServer.ClaimType;
using X.Abp.IdentityServer.Client;
using X.Abp.IdentityServer.Client.Dtos;
using X.Abp.IdentityServer.IdentityResource;
using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.Clients
{
    public partial class CreateModel : IdentityServerPageModel
    {
        protected IClientAppService ClientAppService { get; }

        protected IIdentityResourceAppService IdentityResourceAppService { get; }

        protected IApiResourceAppService ApiResourceAppService { get; }

        protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        [BindProperty]
        public ClientCreateModalView Client { get; set; }

        public ClientSecretModalView SampleSecret { get; set; }

        public List<SelectListItem> SecretTypes { get; set; } = new List<SelectListItem>()
    {
      new SelectListItem()
      {
        Text = "Shared Secret",
        Value = "SharedSecret",
        Selected = true
      },
      new SelectListItem()
      {
        Text = "X509 Thumbprint",
        Value = "X509Thumbprint"
      }
    };

        public string[] AllClaims { get; set; }

        public List<IdentityResourceWithDetailsDto> AllIdentityResources { get; set; } = new List<IdentityResourceWithDetailsDto>();

        public List<ApiResourceWithDetailsDto> AllApiResources { get; set; } = new List<ApiResourceWithDetailsDto>();

        public CreateModel(
          IClientAppService clientAppService,
          IIdentityResourceAppService identityResourceAppService,
          IApiResourceAppService apiResourceAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService)
        {
            ClientAppService = clientAppService;
            IdentityResourceAppService = identityResourceAppService;
            ApiResourceAppService = apiResourceAppService;
            ClaimTypeAppService = claimTypeAppService;
        }

        public virtual async Task OnGetAsync()
        {
            Client = new ClientCreateModalView();
            SampleSecret = new ClientSecretModalView();
            AllIdentityResources = await IdentityResourceAppService.GetAllListAsync();
            AllApiResources = await ApiResourceAppService.GetAllListAsync();
            AllClaims = (await ClaimTypeAppService.GetListAsync()).Select(ct => ct.Name).ToArray();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            Client.TrimUrls();
            CreateClientDto input = ObjectMapper.Map<ClientCreateModalView, CreateClientDto>(Client);
            string[] scopes = Client.Scopes;
            List<string> stringList1 = new List<string>();
            if (scopes != null)
            {
                stringList1 = scopes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            }

            string[] apiResourceScopes = Client.ApiResourceScopes;
            if (apiResourceScopes != null)
            {
                var collection = apiResourceScopes.Where(c => !string.IsNullOrWhiteSpace(c));
                if (collection != null)
                {
                    stringList1.AddRange(collection);
                }
            }

            ClientSecretDto[] secrets = Client.Secrets;
            if (secrets != null)
            {
                var clientSecretDtoArray = secrets.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Value)).ToArray();
                if (clientSecretDtoArray != null)
                {
                    input.Secrets = clientSecretDtoArray;
                }
            }

            input.Scopes = stringList1.ToArray();

            await ClientAppService.CreateAsync(input);
            return NoContent();
        }
    }
}
