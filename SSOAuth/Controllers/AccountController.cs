using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SSOAuth.Business;
using SSOAuth.Business.Interface;
using SSOAuth.Cache;
using SSOAuth.Data.Entity.Authorization;
using SSOAuth.Models.VMs;
using SSOAuth.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SSOAuth.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private IAuthService _authService;
        private IProviderService _providerService;
        IHttpContextAccessor _httpContext;
        private readonly AppSettings _appSetting;
        private readonly ICachingService _cacheServ;
        
        public AccountController(IUserService userService, IAuthService authService, IHttpContextAccessor httpContext,
            IProviderService providerService, AppSettings appSetting, ICachingService cacheServ)
        {
            this._userService = userService;
            this._authService = authService;
            this._httpContext = httpContext;
            this._providerService = providerService;
            this._appSetting = appSetting;
            this._cacheServ = cacheServ;
        }


        //GET: AccountController
        public ActionResult Login(string msg, bool isError = false,string redirectUrl="",string clientId="")
        {
            ViewBag.Message = msg;
            ViewBag.IsError = isError;
            ViewBag.RedirectURL = redirectUrl;
            ViewBag.ClientId = clientId;
            return View(ViewBag);
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserLoginVM user)
        {
            try
            {

                var principle = HttpContext.User as ClaimsPrincipal;
                if(principle.GetIdentityId() != null)
                    return RedirectToAction("Login", new { msg = "Already logged in" });


                //verification of user & pwd
                var res = await _userService.VerifyPassword(user.UserName, user.Password);
                
                if (res != null)
                {
                    
                    var ipAdd = _httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    var expiry = DateTime.Now.AddHours(5);

                    // we get verified user client Id (which system it has logged in from like pharma,radio,lab etc)
                    // and get its provider's info
                    Provider providerData = new Provider();
                    if (!string.IsNullOrEmpty(user.ClientId))
                        providerData = await _providerService.GetProviderByClientId(user.ClientId);
                    else // if there is not client id that means loggin in from current authentication system
                        providerData = await _providerService.GetProviderByClientId(_appSetting.AuthSettings.ClientId);


                    //token is generated on basis of userid , client secret and expiry , scope is caurrently set to 'all'
                    // token will be generated once without depending upon client, multiple clients can use same token
                    // unless it is expired of deactivated
                    var token = await _authService.GenerateToken(res.Id, expiry, providerData.ClientSecret, "all");

                    ////set this token against an authorization code with clientid////
                    ///
                    /// provider will use that code to get token saved ////
                    /// /// 
                    /// 
                    //


                    //logging info of logged in user with token and client is added into respective tables
                    await _userService.EnterLoggedInInfo(res.Id, providerData.Id, token, ipAdd.ToString(), expiry);

                    //set claims in principle
                    await SetLoginCookiesAndClaims(token);



                    var code = await _authService.AuthorizationCodeGeneration(providerData, token, res.Identity);
                    //https://provider.domain.com/auth?code=4/P7q7W91a-oMsCeLvIaQm6bTrgtp7&Redirect={clientPage}
                    // also see url of /signin-oidc
                    //redirect to domain
                    if (!string.IsNullOrEmpty(user.RedirectURL))
                        return Redirect(_authService.GetCallbackPath(providerData.Domain, code, user.RedirectURL));

                    return RedirectToAction("Login", new { msg = "Successfully logged in" });
                }
                else 
                {
                    //if (!string.IsNullOrEmpty(user.RedirectURL))
                    //{
                    //    return Redirect(user.RedirectURL);
                    //}
                    return RedirectToAction("Login", new { msg = "No user found", isError = true });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", new { msg = ex.Message ,isError = true  });
            }
        }


        private async Task<bool> SetLoginCookiesAndClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedValue = handler.ReadJwtToken(token);


            var claimsIdentity = new ClaimsIdentity(
  decodedValue.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(claimsIdentity),
              authProperties);
            return true;
        }


        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(
      CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", new { msg = "Logged out", isError = false });
        }
    }
}
