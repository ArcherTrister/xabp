// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.CmsKit.Contact;

public class ContactConfig
{
    public string ReceiverEmailAddress { get; }

    public ContactConfig(string receiverEmailAddress)
    {
        ReceiverEmailAddress = receiverEmailAddress;
    }
}
