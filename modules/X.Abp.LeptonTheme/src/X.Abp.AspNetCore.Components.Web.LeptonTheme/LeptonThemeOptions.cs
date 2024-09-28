// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme
{
    public class LeptonThemeOptions
    {
        public Type FooterComponent { get; set; }

        /// <summary>
        /// Set this to use a custom style file. It should be a full path.
        /// THIS OPTION ONLY WORKS FOR BLAZOR SERVER SIDE
        /// </summary>
        public string StylePath { get; set; }
    }
}
