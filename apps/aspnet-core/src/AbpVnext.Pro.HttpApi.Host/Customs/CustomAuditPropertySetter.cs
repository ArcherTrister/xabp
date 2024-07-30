//// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
//// See https://github.com/ArcherTrister/xabp
//// for more information concerning the license and the contributors participating to this project.

//using Volo.Abp;
//using Volo.Abp.Auditing;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.Timing;
//using Volo.Abp.Users;

//namespace AbpVnext.Pro.Customs;

//[Dependency(ReplaceServices = true)]
//[ExposeServices(typeof(IAuditPropertySetter))]
//public class CustomAuditPropertySetter : AuditPropertySetter, IAuditPropertySetter, ITransientDependency
//{
//    public CustomAuditPropertySetter(
//        ICurrentUser currentUser,
//        ICurrentTenant currentTenant,
//        IClock clock)
//        : base(currentUser, currentTenant, clock)
//    {
//    }

//    public override void SetCreationProperties(object targetObject)
//    {
//        SetCreationTime(targetObject);
//        SetCreatorId(targetObject);
//        SetCreatorName(targetObject);
//    }

//    public override void SetModificationProperties(object targetObject)
//    {
//        SetLastModificationTime(targetObject);
//        SetLastModifierId(targetObject);
//        SetLastModifierName(targetObject);
//    }

//    public override void SetDeletionProperties(object targetObject)
//    {
//        SetDeletionTime(targetObject);
//        SetDeleterId(targetObject);
//        SetDeleterName(targetObject);
//    }

//    protected override void SetCreatorId(object targetObject)
//    {
//        if (!CurrentUser.Id.HasValue)
//        {
//            return;
//        }

//        /*
//        if (targetObject is IMultiTenant multiTenantEntity)
//        {
//            if (multiTenantEntity.TenantId != CurrentUser.TenantId)
//            {
//                return;
//            }
//        }
//        */

//        if (targetObject is IMayHaveCreator mayHaveCreatorObject)
//        {
//            if (mayHaveCreatorObject.CreatorId.HasValue && mayHaveCreatorObject.CreatorId.Value != default)
//            {
//                return;
//            }

//            ObjectHelper.TrySetProperty(mayHaveCreatorObject, x => x.CreatorId, () => CurrentUser.Id);
//        }
//        else if (targetObject is IMustHaveCreator mustHaveCreatorObject)
//        {
//            if (mustHaveCreatorObject.CreatorId != default)
//            {
//                return;
//            }

//            ObjectHelper.TrySetProperty(mustHaveCreatorObject, x => x.CreatorId, () => CurrentUser.Id.Value);
//        }
//    }

//    private void SetCreatorName(object targetObject)
//    {
//        if (string.IsNullOrWhiteSpace(CurrentUser.Name))
//        {
//            return;
//        }

//        /*
//        if (targetObject is IMultiTenant multiTenantEntity)
//        {
//            if (multiTenantEntity.TenantId != CurrentUser.TenantId)
//            {
//                return;
//            }
//        }
//        */

//        if (targetObject is IMayHaveCreatorSurName mayHaveCreatorSurNameObject)
//        {
//            if (!string.IsNullOrWhiteSpace(mayHaveCreatorSurNameObject.Creator) && mayHaveCreatorSurNameObject.Creator != default)
//            {
//                return;
//            }

//            ObjectHelper.TrySetProperty(mayHaveCreatorSurNameObject, x => x.Creator, () => CurrentUser.SurName + CurrentUser.Name);
//        }
//    }

//    private void SetLastModifierName(object targetObject)
//    {
//        if (!(targetObject is IModificationSurName modificationSurNameObject))
//        {
//            return;
//        }

//        if (string.IsNullOrWhiteSpace(CurrentUser.Name))
//        {
//            ObjectHelper.TrySetProperty(modificationSurNameObject, x => x.LastModifier, () => null);
//            return;
//        }

//        if (modificationSurNameObject is IMultiTenant multiTenantEntity)
//        {
//            if (multiTenantEntity.TenantId != CurrentUser.TenantId)
//            {
//                ObjectHelper.TrySetProperty(modificationSurNameObject, x => x.LastModifier, () => null);
//                return;
//            }
//        }

//        ObjectHelper.TrySetProperty(modificationSurNameObject, x => x.LastModifier, () => CurrentUser.SurName + CurrentUser.Name);
//    }

//    private void SetDeleterName(object targetObject)
//    {
//        if (!(targetObject is IDeletionSurName deletionSurNameObject))
//        {
//            return;
//        }

//        if (string.IsNullOrWhiteSpace(CurrentUser.Name))
//        {
//            ObjectHelper.TrySetProperty(deletionSurNameObject, x => x.Deleter, () => null);
//            return;
//        }

//        if (deletionSurNameObject is IMultiTenant multiTenantEntity)
//        {
//            if (multiTenantEntity.TenantId != CurrentUser.TenantId)
//            {
//                ObjectHelper.TrySetProperty(deletionSurNameObject, x => x.Deleter, () => null);
//                return;
//            }
//        }

//        ObjectHelper.TrySetProperty(deletionSurNameObject, x => x.Deleter, () => CurrentUser.SurName + CurrentUser.Name);
//    }
//}
