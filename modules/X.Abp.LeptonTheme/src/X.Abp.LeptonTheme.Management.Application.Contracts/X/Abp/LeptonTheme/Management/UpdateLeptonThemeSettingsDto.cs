// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.LeptonTheme.Management
{
    public class UpdateLeptonThemeSettingsDto
    {
        public bool BoxedLayout { get; set; }

        public MenuPlacement MenuPlacement { get; set; }

        public MenuStatus MenuStatus { get; set; }

        public LeptonStyle Style { get; set; }

        public LeptonStyle PublicLayoutStyle { get; set; }
    }
}
