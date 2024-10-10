# abp-payment

Payment module for ABP framework.

[Payment Module (Pro)](https://abp.io/docs/latest/modules/payment)

## Supported Gateway Packages

| Package Name | Stable |
|:-|:-:|
|X.Abp.Payment.WeChatPay.Domain|[![NuGet](https://img.shields.io/nuget/v/X.Abp.Payment.WeChatPay.Domain?logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/X.Abp.Payment.WeChatPay.Domain/ "Download X.Abp.Payment.WeChatPay.Domain from NuGet.org")|
|X.Abp.Payment.WeChatPay.Domain.Shared|[![NuGet](https://img.shields.io/nuget/v/X.Abp.Payment.WeChatPay.Domain.Shared?logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/X.Abp.Payment.WeChatPay.Domain.Shared/ "Download X.Abp.Payment.WeChatPay.Domain.Shared from NuGet.org")|
|X.Abp.Payment.WeChatPay.Web|[![NuGet](https://img.shields.io/nuget/v/X.Abp.Payment.WeChatPay.Web?logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/X.Abp.Payment.WeChatPay.Web/ "Download X.Abp.Payment.WeChatPay.Web from NuGet.org")|

## Options

### WeChatPayOptions

```WeChatPayOptions``` is used to configure WeChatPay payment gateway options。 **WeChatPay gateway only supports CNY currency**.

* ```AppId```：应用号。公众号、移动应用、小程序AppId、企业微信CorpId。
* ```AppSecret```: 应用密钥。企业微信AppSecret，目前仅调用"企业红包API"时使用。
* ```MchId```: 商户号。商户号、服务商户号。
* ```SubAppId```: 子商户应用号。目前仅调用服务商API时使用，子商户的公众号、移动应用AppId。
* ```SubMchId```: 子商户号。目前仅调用服务商API时使用，子商户的商户号。
* ```Certificate```商户API证书。可为证书文件路径、证书文件的Base64编码。
* ```CertificatePassword```: 商户API证书密码。默认为商户号。
* ```APIPrivateKey```: 商户API私钥。当配置了P12格式证书时，已包含私钥信息，不必再配置API私钥。PEM格式则必须配置。
* ```APIKey```: 商户API密钥。
* ```APIv3Key```: 商户APIv3密钥。
* ```RsaPublicKey```: RSA公钥。目前仅调用"企业付款到银行卡API [V2]"时使用，执行"获取RSA加密公钥API [V2]"即可获取。

#### WeChatPayWebOptions

* ```Recommended```: Is payment gateway is recommended or not. This information is displayed on payment gateway selection page.
* ```ExtraInfos```: List of informative strings for payment gateway. These texts are displayed on payment gateway selection page.
* ```PrePaymentCheckoutButtonStyle```: CSS style to add Checkout button on Iyzico prepayment page. This class can be used for tracking user activity via 3rd party tools like Google Tag Manager.

> You can check the [WeChatPay V2 document](https://pay.weixin.qq.com/wiki/doc/api/index.html) for more details.

> You can check the [WeChatPay V3 document](https://pay.weixin.qq.com/wiki/doc/apiv3/index.shtml) for more details.

Instead of configuring options in your module class, you can configure it in your appsettings.json file like below;

```json
"Payment": {
    "WeChatPay": {
    // 应用号
    // 如：微信公众平台AppId、微信开放平台AppId、微信小程序AppId、企业微信CorpId等
    "AppId": "APP_ID",

    "AppSecret": "APP_SECRET",
    // 商户号
    // 为微信支付商户平台的商户号
    "MchId": "MCH_ID",

    // 商户API密钥
    // 为微信支付商户平台的API密钥，请注意不是APIv3密钥
    "APIKey": "API_KEY",

    // 商户APIv3密钥
    // 为微信支付商户平台的APIv3密钥，请注意不是API密钥，v3接口必填
    "APIv3Key": "API_V3_KEY",

    // 商户API证书
    // 使用V2退款、付款等接口时必填
    // 使用V3接口时必填
    // 可为证书文件路径 / 证书文件的base64字符串
    "Certificate": "apiclient_cert.p12",

    // 商户API私钥
    // 当配置了P12格式证书时，已包含私钥，不必再单独配置API私钥。
    // PEM格式证书，需要单独配置。
    "APIPrivateKey": "",

    // RSA公钥
    // 目前仅调用"企业付款到银行卡API [V2]"时使用，执行本示例中的"获取RSA加密公钥API [V2]"即可获取。
    "RsaPublicKey": ""
    }
  }
```
