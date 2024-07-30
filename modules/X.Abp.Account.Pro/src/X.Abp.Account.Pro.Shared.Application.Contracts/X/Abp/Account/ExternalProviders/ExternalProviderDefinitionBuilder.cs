// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

using Volo.Abp;

namespace X.Abp.Account.ExternalProviders;

public class ExternalProviderDefinitionBuilder<TOptions>
{
    public string AuthenticationSchema { get; }

    public List<ExternalProviderDefinitionProperty> Properties { get; }

    internal ExternalProviderDefinitionBuilder(string authenticationSchema)
    {
        AuthenticationSchema = authenticationSchema;
        Properties = new List<ExternalProviderDefinitionProperty>();
    }

    public ExternalProviderDefinitionBuilder<TOptions> WithProperty<TProperty>(
        [NotNull] Expression<Func<TOptions, TProperty>> propertySelector,
        bool isSecret = false)
    {
        Check.NotNull(propertySelector, nameof(propertySelector));

        if (propertySelector.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException($"{nameof(propertySelector)} must be a property expression!");
        }

        var propertyInfo = memberExpression.Member as PropertyInfo;
        if (propertyInfo == null)
        {
            throw new ArgumentException($"{nameof(propertySelector)} must be a property expression!");
        }

        Properties.Add(new ExternalProviderDefinitionProperty
        {
            PropertyName = propertyInfo.Name,
            IsSecret = isSecret
        });

        return this;
    }

    internal ExternalProviderDefinition Build()
    {
        return new ExternalProviderDefinition
        {
            Name = AuthenticationSchema,
            Properties = Properties
        };
    }
}
