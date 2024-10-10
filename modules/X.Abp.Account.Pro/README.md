# abp-account-pro

Account Pro module for ABP framework.

[Account Module (Pro)](https://abp.io/docs/latest/modules/account-pro)

### IdentityServer4解决 claims 丢失
- 1 重写 AbpProfileService[推荐]
- 2 添加 claim 到 userclaim

### 客户端与设备映射
- IdentityServer4设置
```csharp
Configure<AbpAccountIdentityServerOptions>(options =>
{
    options.ClientIdToDeviceMap.Add("clientId", "device");
});
```

- OpenIddict设置
```csharp
Configure<AbpAccountOpenIddictOptions>(options =>
{
    options.ClientIdToDeviceMap.Add("clientId", "device");
});
```
