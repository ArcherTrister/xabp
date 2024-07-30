// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity.Web.Pages.Identity.Users;

public class OrganizationUnitTreeNodeModel
{
    public IdentityUserModalPageModel.AssignedOrganizationUnitViewModel[] OrganizationUnits { get; set; }

    public int Depth { get; set; }

    public IdentityUserModalPageModel.OrganizationTreeNode Node { get; set; }
}
