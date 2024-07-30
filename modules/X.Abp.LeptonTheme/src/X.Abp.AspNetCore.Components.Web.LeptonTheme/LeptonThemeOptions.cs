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
