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

```WeChatPayOptions``` is used to configure WeChatPay payment gateway options�� **WeChatPay gateway only supports CNY currency**.

* ```AppId```��Ӧ�úš����ںš��ƶ�Ӧ�á�С����AppId����ҵ΢��CorpId��
* ```AppSecret```: Ӧ����Կ����ҵ΢��AppSecret��Ŀǰ������"��ҵ���API"ʱʹ�á�
* ```MchId```: �̻��š��̻��š������̻��š�
* ```SubAppId```: ���̻�Ӧ�úš�Ŀǰ�����÷�����APIʱʹ�ã����̻��Ĺ��ںš��ƶ�Ӧ��AppId��
* ```SubMchId```: ���̻��š�Ŀǰ�����÷�����APIʱʹ�ã����̻����̻��š�
* ```Certificate```�̻�API֤�顣��Ϊ֤���ļ�·����֤���ļ���Base64���롣
* ```CertificatePassword```: �̻�API֤�����롣Ĭ��Ϊ�̻��š�
* ```APIPrivateKey```: �̻�API˽Կ����������P12��ʽ֤��ʱ���Ѱ���˽Կ��Ϣ������������API˽Կ��PEM��ʽ��������á�
* ```APIKey```: �̻�API��Կ��
* ```APIv3Key```: �̻�APIv3��Կ��
* ```RsaPublicKey```: RSA��Կ��Ŀǰ������"��ҵ������п�API [V2]"ʱʹ�ã�ִ��"��ȡRSA���ܹ�ԿAPI [V2]"���ɻ�ȡ��

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
    // Ӧ�ú�
    // �磺΢�Ź���ƽ̨AppId��΢�ſ���ƽ̨AppId��΢��С����AppId����ҵ΢��CorpId��
    "AppId": "APP_ID",

    "AppSecret": "APP_SECRET",
    // �̻���
    // Ϊ΢��֧���̻�ƽ̨���̻���
    "MchId": "MCH_ID",

    // �̻�API��Կ
    // Ϊ΢��֧���̻�ƽ̨��API��Կ����ע�ⲻ��APIv3��Կ
    "APIKey": "API_KEY",

    // �̻�APIv3��Կ
    // Ϊ΢��֧���̻�ƽ̨��APIv3��Կ����ע�ⲻ��API��Կ��v3�ӿڱ���
    "APIv3Key": "API_V3_KEY",

    // �̻�API֤��
    // ʹ��V2�˿����Ƚӿ�ʱ����
    // ʹ��V3�ӿ�ʱ����
    // ��Ϊ֤���ļ�·�� / ֤���ļ���base64�ַ���
    "Certificate": "apiclient_cert.p12",

    // �̻�API˽Կ
    // ��������P12��ʽ֤��ʱ���Ѱ���˽Կ�������ٵ�������API˽Կ��
    // PEM��ʽ֤�飬��Ҫ�������á�
    "APIPrivateKey": "",

    // RSA��Կ
    // Ŀǰ������"��ҵ������п�API [V2]"ʱʹ�ã�ִ�б�ʾ���е�"��ȡRSA���ܹ�ԿAPI [V2]"���ɻ�ȡ��
    "RsaPublicKey": ""
    }
  }
```
