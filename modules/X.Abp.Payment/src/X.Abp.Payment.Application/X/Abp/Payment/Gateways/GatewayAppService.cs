// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace X.Abp.Payment.Gateways;

public class GatewayAppService :
PaymentAppServiceBase,
IGatewayAppService
{
    protected IOptions<PaymentOptions> PaymentOptions { get; }

    public GatewayAppService(IOptions<PaymentOptions> paymentOptions)
    {
        PaymentOptions = paymentOptions;
    }

    public virtual Task<List<GatewayDto>> GetGatewayConfigurationAsync()
    {
        return Task.FromResult(PaymentOptions.Value.Gateways.Select(keyValuePair => new GatewayDto()
        {
            Name = keyValuePair.Value.Name,
            DisplayName = (string)keyValuePair.Value.DisplayName.Localize(StringLocalizerFactory)
        }).ToList());
    }

    public virtual Task<List<GatewayDto>> GetSubscriptionSupportedGatewaysAsync()
    {
        return Task.FromResult(PaymentOptions.Value.Gateways.Where(g => g.Value.IsSubscriptionSupported)
            .Select(keyValuePair => new GatewayDto()
            {
                Name = keyValuePair.Value.Name,
                DisplayName = (string)keyValuePair.Value.DisplayName.Localize(StringLocalizerFactory)
            }).ToList());
    }
}
