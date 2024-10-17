// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.CmsKit.Admin.Newsletters;

public class ImportNewslettersFromFileDto
{
    public string EmailAddress { get; set; }

    public string Preference { get; set; }

    public string Source { get; set; }

    public string SourceUrl { get; set; }

    public override string ToString()
    {
        return $"EmailAddress: {EmailAddress}, Preference: {Preference}, Source: {Source}, SourceUrl: {SourceUrl}";
    }
}
