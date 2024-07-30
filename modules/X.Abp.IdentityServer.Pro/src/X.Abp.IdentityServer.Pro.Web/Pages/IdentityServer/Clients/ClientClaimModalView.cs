// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Mvc;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.Clients
{
    public class ClientClaimModalView
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public Guid ClientId { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
