// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Payment.Gateways;

public class PaymentGatewayResolver : IScopedDependency
{
    protected IServiceProvider ServiceProvider { get; }

    protected PaymentOptions PaymentOptions { get; }

    protected ILogger<PaymentGatewayResolver> Logger { get; }

    public PaymentGatewayResolver(
      IOptions<PaymentOptions> paymentOptions,
      ILogger<PaymentGatewayResolver> logger,
      IServiceProvider serviceProvider)
    {
        PaymentOptions = paymentOptions.Value;
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    public virtual IPaymentGateway Resolve(string gatewayName)
    {
        var gatewayConfiguration = PaymentOptions.Gateways.FirstOrDefault(x => x.Value.Name.Equals(gatewayName, StringComparison.Ordinal)).Value;

        return gatewayConfiguration == null
            ? throw new ArgumentException("Payment gateway with name " + gatewayName + " not found.", nameof(gatewayName))
            : ServiceProvider.GetRequiredService(gatewayConfiguration.PaymentGatewayType) is IPaymentGateway requiredService
            ? requiredService
            : throw new InvalidOperationException("'PaymentGatewayType' of PaymentOptions isn't configured properly. That type must implement 'IPaymentGateway'");
    }
}
