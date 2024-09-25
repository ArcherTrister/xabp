// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

using X.Abp.Payment.Requests;

namespace X.Abp.Payment.WeChatPay;

public class PurchaseParameterListGenerator : IPurchaseParameterListGenerator, ITransientDependency
{
    private readonly IJsonSerializer _jsonSerializer;
    public PurchaseParameterListGenerator(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public WeChatPayPaymentRequestExtraParameterConfiguration GetExtraParameterConfiguration(PaymentRequest paymentRequest)
    {
        WeChatPayPaymentRequestExtraParameterConfiguration propertyConfiguration = new WeChatPayPaymentRequestExtraParameterConfiguration();
        if (!paymentRequest.ExtraProperties.TryGetValue("WeChatPay", out var _))
        {
            return propertyConfiguration;
        }

        WeChatPayPaymentRequestExtraParameterConfiguration parameterConfiguration = _jsonSerializer.Deserialize<WeChatPayPaymentRequestExtraParameterConfiguration>(paymentRequest.ExtraProperties["WeChatPay"].ToString());
        //if (!parameterConfiguration.AdditionalCallbackParameters.IsNullOrEmpty())
        //{
        //    propertyConfiguration.AdditionalCallbackParameters = parameterConfiguration.AdditionalCallbackParameters;
        //}

        return propertyConfiguration;
    }
}
