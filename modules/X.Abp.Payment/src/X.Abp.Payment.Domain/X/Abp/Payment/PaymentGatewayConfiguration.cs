// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Localization;

namespace X.Abp.Payment;

public class PaymentGatewayConfiguration
{
    private ILocalizableString localizableString;

    public string Name { get; }

    public ILocalizableString DisplayName
    {
        get => localizableString;
        set => localizableString = Check.NotNull(value, nameof(value));
    }

    public Type PaymentGatewayType { get; }

    public int Order { get; set; } = 1000;

    public bool IsSubscriptionSupported { get; }

    public PaymentGatewayConfiguration(
      string name,
      ILocalizableString displayName,
      bool isSubscriptionSupported,
      Type paymentGatewayType)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        DisplayName = displayName;
        IsSubscriptionSupported = isSubscriptionSupported;
        PaymentGatewayType = paymentGatewayType;
    }
}
