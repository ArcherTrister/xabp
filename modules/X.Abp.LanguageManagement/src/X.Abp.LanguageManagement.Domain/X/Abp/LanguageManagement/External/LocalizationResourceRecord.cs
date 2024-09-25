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

        protected LocalizationResourceRecord()
        {
        }

        public LocalizationResourceRecord(LocalizationResourceBase resource, IEnumerable<string> supportedCultures)
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

        public bool TryUpdate(LocalizationResourceBase resource, IEnumerable<string> supportedCultures)
        {
            return IsUpdateDefaultCulture(resource) | IsUpdateBaseResources(resource) | IsUpdateSupportedCultures(supportedCultures);
        }

        private bool IsUpdateDefaultCulture(LocalizationResourceBase localizationResource)
        {
            if (DefaultCulture == localizationResource.DefaultCultureName)
            {
                return false;
            }

            DefaultCulture = localizationResource.DefaultCultureName;
            return true;
        }

        private bool IsUpdateBaseResources(LocalizationResourceBase localizationResource)
        {
            string str = Transform(localizationResource.BaseResourceNames);
            if (BaseResources == str)
            {
                return false;
            }

            BaseResources = str;
            return true;
        }

        private bool IsUpdateSupportedCultures(IEnumerable<string> supportedCultures)
        {
            string str = Transform(supportedCultures);
            if (SupportedCultures == str)
            {
                return false;
            }

            SupportedCultures = str;
            return true;
        }

        private static string Transform(IEnumerable<string> supportedCultures)
        {
            string str = string.Join(",", supportedCultures);
            return str.IsNullOrWhiteSpace() ? null : str;
        }

        private static string[] Transform(string str)
        {
            string[] strArray;
            if (str == null)
            {
                strArray = null;
            }
            else
            {
                strArray = str.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (strArray != null)
                {
                    return strArray;
                }
            }

            return Array.Empty<string>();
        }
    }
}
