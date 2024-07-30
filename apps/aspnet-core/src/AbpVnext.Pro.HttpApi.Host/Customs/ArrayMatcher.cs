// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.Http.Modeling;
using Volo.Abp.Reflection;
using Volo.Abp.Threading;

namespace AbpVnext.Pro.Customs;

internal static class ArrayMatcher
{
    public static T[] Match<T>(T[] sourceArray, T[] destinationArray)
    {
        var result = new List<T>();

        var currentMethodParamIndex = 0;
        var parentItem = default(T);

        foreach (var sourceItem in sourceArray)
        {
            if (currentMethodParamIndex < destinationArray.Length)
            {
                var destinationItem = destinationArray[currentMethodParamIndex];

                if (EqualityComparer<T>.Default.Equals(sourceItem, destinationItem))
                {
                    parentItem = default;
                    currentMethodParamIndex++;
                }
                else
                {
                    if (parentItem == null)
                    {
                        parentItem = destinationItem;
                        currentMethodParamIndex++;
                    }
                }
            }

            var resultItem = EqualityComparer<T>.Default.Equals(parentItem, default) ? sourceItem : parentItem;
            result.Add(resultItem);
        }

        return result.ToArray();
    }
}

// public static class ApiDescriptionExtensions
// {
//    public static bool IsRemoteService(this ApiDescription apiDescriptor)
//    {
//        if (apiDescriptor.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
//        {
//            var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<RemoteServiceAttribute>(controllerActionDescriptor.MethodInfo);
//            if (remoteServiceAttr != null && remoteServiceAttr.IsEnabled)
//            {
//                return true;
//            }

// remoteServiceAttr = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<RemoteServiceAttribute>(controllerActionDescriptor.ControllerTypeInfo);
//            if (remoteServiceAttr != null && remoteServiceAttr.IsEnabled)
//            {
//                return true;
//            }

// if (typeof(IRemoteService).IsAssignableFrom(controllerActionDescriptor.ControllerTypeInfo))
//            {
//                return true;
//            }
//        }

// return false;
//    }

// public static bool IsIntegrationService(this ApiDescription apiDescriptor)
//    {
//        if (apiDescriptor.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
//        {
//            return IntegrationServiceAttribute.IsDefinedOrInherited(controllerActionDescriptor.ControllerTypeInfo);
//        }

// return false;
//    }
// }
