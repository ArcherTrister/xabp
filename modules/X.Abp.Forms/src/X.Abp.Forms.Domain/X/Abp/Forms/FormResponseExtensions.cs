// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Data;

using X.Abp.Forms.Responses;

namespace X.Abp.Forms;

public static class FormResponseExtensions
{
    private const string EmailPropertyName = "Email";

    public static void SetEmail(this FormResponse response, string email)
    {
        response.SetProperty(EmailPropertyName, email);
    }

    public static string GetEmail(this FormResponse response)
    {
        return response.GetProperty<string>(EmailPropertyName);
    }
}
