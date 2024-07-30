// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp;

namespace X.Abp.Payment.Web;

public class PaymentGatewayWebConfiguration
{
    public string Name { get; }

    public string PrePaymentUrl { get; }

    public string PostPaymentUrl { get; }

    public int Order { get; set; } = 1000;

    public bool Recommended { get; set; }

    public List<string> ExtraInfos { get; set; }

    public PaymentGatewayWebConfiguration(
      string name,
      string prePaymentUrl,
      string postPaymentUrl,
      bool recommended = false,
      List<string> extraInfos = null)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        PrePaymentUrl = Check.NotNull(prePaymentUrl, nameof(prePaymentUrl));
        PostPaymentUrl = postPaymentUrl;
        Recommended = recommended;
        ExtraInfos = extraInfos ?? new List<string>();
    }
}
