// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.LeptonTheme.Management.Settings
{
    public static class LeptonThemeSettingNames
    {
        private const string DefaultPrefix = "Volo.Abp.LeptonTheme";

        public const string Style = DefaultPrefix + ".Style";
        public const string PublicLayoutStyle = DefaultPrefix + ".Style.PublicLayout";

        public static class Layout
        {
            private const string LayoutPrefix = DefaultPrefix + ".Layout";

            public const string Boxed = LayoutPrefix + ".Boxed";

            public const string MenuPlacement = LayoutPrefix + ".MenuPlacement";

            public const string MenuStatus = LayoutPrefix + ".MenuStatus";
        }
    }
}
