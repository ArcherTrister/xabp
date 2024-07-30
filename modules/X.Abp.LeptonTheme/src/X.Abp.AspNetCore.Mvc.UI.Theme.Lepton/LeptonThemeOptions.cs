// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton
{
    public class LeptonThemeOptions
    {
        /// <summary>
        /// Enables Lepton Theme demo features. When set to false, it doesn't add any extra html or javascript to your web application.
        /// </summary>
        public bool EnableDemoFeatures { get; set; }

        public bool IsPublicWebsite { get; set; }

        /// <summary>
        /// Set this to use a custom style file. It should be a full path.
        /// </summary>
        public string StylePath { get; set; }
    }
}
