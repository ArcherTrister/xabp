// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Localization;

namespace X.Abp.LanguageManagement.External
{
    public class LocalizationResourceRecord
        : BasicAggregateRoot<Guid>,
            IHasCreationTime,
            IHasModificationTime
    {
        public virtual string Name { get; private set; }

        public virtual string DefaultCulture { get; set; }

        public virtual string BaseResources { get; private set; }

        public virtual string SupportedCultures { get; private set; }

        public virtual DateTime CreationTime { get; protected set; }

        public DateTime? LastModificationTime { get; set; }

        protected LocalizationResourceRecord() { }

        public LocalizationResourceRecord(
            LocalizationResourceBase resource,
            IEnumerable<string> supportedCultures
        )
        {
            Name = Check.NotNullOrWhiteSpace(resource.ResourceName, nameof(resource.ResourceName));

            DefaultCulture = resource.DefaultCultureName;
            SetBaseResources(resource.BaseResourceNames);
            SetSupportedCultures(supportedCultures);
        }

        public string[] GetBaseResources() => Transform(BaseResources);

        private LocalizationResourceRecord SetBaseResources(IEnumerable<string> baseResources)
        {
            BaseResources = Transform(baseResources);
            return this;
        }

        public string[] GetSupportedCultures() => Transform(SupportedCultures);

        private LocalizationResourceRecord SetSupportedCultures(IEnumerable<string> supportedCultures)
        {
            SupportedCultures = Transform(supportedCultures);
            return this;
        }

        public bool TryUpdate(
            LocalizationResourceBase resource,
            IEnumerable<string> supportedCultures
        )
        {
            return method_5(resource) | method_6(resource) | method_7(supportedCultures);
        }

        private bool method_5(LocalizationResourceBase localizationResource)
        {
            if (DefaultCulture == localizationResource.DefaultCultureName)
            {
                return false;
            }

            DefaultCulture = localizationResource.DefaultCultureName;
            return true;
        }

        private bool method_6(LocalizationResourceBase localizationResource)
        {
            string str = Transform(localizationResource.BaseResourceNames);
            if (BaseResources == str)
            {
                return false;
            }

            BaseResources = str;
            return true;
        }

        private bool method_7(IEnumerable<string> ienumerable_0)
        {
            string str = Transform(ienumerable_0);
            if (SupportedCultures == str)
            {
                return false;
            }

            SupportedCultures = str;
            return true;
        }

        private static string Transform(IEnumerable<string> ienumerable_0)
        {
            string str = string.Join(",", ienumerable_0);
            return str.IsNullOrWhiteSpace() ? null : str;
        }

        private string[] Transform(string str)
        {
            string[] strArray;
            if (str == null)
            {
                strArray = null;
            }
            else
            {
                strArray = str.Split(
                    ",",
                    StringSplitOptions.RemoveEmptyEntries
                );
                if (strArray != null)
                {
                    return strArray;
                }
            }
            return Array.Empty<string>();
        }
    }
}
