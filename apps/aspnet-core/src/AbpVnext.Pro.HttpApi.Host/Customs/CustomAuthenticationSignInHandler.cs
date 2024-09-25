//using System.Security.Claims;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//using OpenIddict.Server.AspNetCore;

//namespace AbpVnext.Pro.Customs;

//public class CustomAuthenticationSignInHandler : OpenIddictServerAspNetCoreHandler// AuthenticationHandler<OpenIddictServerAspNetCoreOptions>, IAuthenticationSignInHandler
//{
//    public CustomAuthenticationSignInHandler(IOptionsMonitor<OpenIddictServerAspNetCoreOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
//    {
//    }

//    public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
//    {
//        throw new System.NotImplementedException();
//    }

//    public Task SignOutAsync(AuthenticationProperties properties)
//    {
//        throw new System.NotImplementedException();
//    }

//    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//    {
//        throw new System.NotImplementedException();
//    }
//}
