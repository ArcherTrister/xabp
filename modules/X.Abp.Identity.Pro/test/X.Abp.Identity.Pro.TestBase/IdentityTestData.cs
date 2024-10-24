﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Identity;

public class IdentityTestData : ISingletonDependency
{
    public Guid RoleModeratorId { get; } = Guid.NewGuid();
    public Guid RoleSupporterId { get; } = Guid.NewGuid();
    public Guid RoleManagerId { get; } = Guid.NewGuid();
    public Guid RoleSaleId { get; } = Guid.NewGuid();
    public Guid UserJohnId { get; } = Guid.NewGuid();
    public Guid UserDavidId { get; } = Guid.NewGuid();
    public Guid UserNeoId { get; } = Guid.NewGuid();
    public Guid UserBobId { get; } = Guid.NewGuid();
    public Guid AgeClaimId { get; } = Guid.NewGuid();
    public Guid EducationClaimId { get; } = Guid.NewGuid();
}
