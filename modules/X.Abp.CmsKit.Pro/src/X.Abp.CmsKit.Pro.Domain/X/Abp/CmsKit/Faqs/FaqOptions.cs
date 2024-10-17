// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;

using Volo.Abp;

namespace X.Abp.CmsKit.Faqs
{
    public class FaqOptions
    {
        private FrozenDictionary<string, FaqGroupInfo> frozenDictionary;

        public IReadOnlyDictionary<string, FaqGroupInfo> Groups
        {
            get
            {
                FrozenDictionary<string, FaqGroupInfo> groups = frozenDictionary;
                if (groups != null)
                {
                    return groups;
                }

                frozenDictionary = new Dictionary<string, FaqGroupInfo>().ToFrozenDictionary();
                return frozenDictionary;
            }
        }

        public FaqOptions()
        {
            frozenDictionary = new Dictionary<string, FaqGroupInfo>().ToFrozenDictionary();
        }

        public virtual void SetGroups(ReadOnlySpan<string> names)
        {
            Dictionary<string, FaqGroupInfo> source = new Dictionary<string, FaqGroupInfo>();

            for (int index = 0; index < names.Length; ++index)
            {
                string name = names[index];
                Check.NotNullOrWhiteSpace(name, nameof(name));
                if (HasGroup(name))
                {
                    throw new InvalidOperationException(name);
                }

                source.Add(name, new FaqGroupInfo(name));
            }

            frozenDictionary = source.ToFrozenDictionary((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
        }

        public virtual bool HasGroup(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            return Groups.ContainsKey(name);
        }
    }
}
