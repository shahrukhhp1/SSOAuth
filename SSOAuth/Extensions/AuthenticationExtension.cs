using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SSOAuth.Models.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SSOAuth
{
    public static class AuthenticationExtension
    {
        public static async Task<MiddlewareAuthDTO> CustomAuthenticate(this HttpContext _httpContext)
        {
            MiddlewareAuthDTO ret = new MiddlewareAuthDTO();
            var loginPath = "/account/login";
            var authMatch = "oauth2";
            
            var request = _httpContext.Request;
            var response = _httpContext.Response;


            if (request.Path == loginPath || request.Path.Value.Contains(authMatch))
            {
                ret.IsAuthenticated = true;
            }
            else 
            {
                var authentication = _httpContext.User.GetIdentityId(); //GetCookieValue(_httpContext.Request, HeaderNames.Authorization);
                if (authentication == null) 
                {
                    ret.IsAuthenticated = false;
                    ret.RedirectURL = loginPath;
                    ret.IsPermenantRedirect = false;
                }
                else 
                {
                    var token = authentication.Replace("Bearer ", "");
                    //validateToken(token,)
                    ret.IsAuthenticated = true;
                    ret.IsPermenantRedirect = false;
                }
            }
            return ret;
        }

        public static string GetIdentityId(this ClaimsPrincipal claimsId)
        {
            var id = claimsId.Claims.Where(x => x.Type.Equals("IdentityId")).FirstOrDefault();
            if (id == null)
                return null;

            return id.Value;
        }


        private static string GetCookieValue(HttpRequest request, string cookieName)
        {
            foreach (var cook in request.Cookies)
            {
                if (cook.Key != cookieName)
                    continue;
                return cook.Value;
            }
            return null;
        }



        public static Task<IPrincipal> validateToken(string token, string issuer, string audience, string secret)
        {
            ClaimsPrincipal principal = getPrincipal(token,issuer,audience,secret);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
                IPrincipal Iprincipal = new ClaimsPrincipal(identity);
                return Task.FromResult(Iprincipal);
            }
            catch (NullReferenceException)
            {
                return Task.FromResult<IPrincipal>(null);
            }
        }

        private static ClaimsPrincipal getPrincipal(string token,string issuer,string audience,string secret)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Encoding.ASCII.GetBytes(secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }



        //private static string GetCookieValue(HttpRequest request, string cookieName)
        //{
        //    foreach (var headers in request.Headers)
        //    {
        //        if (headers.Key != "Set-Cookie")
        //            continue;
        //        string header = headers.Value;
        //        if (header.StartsWith($"{cookieName}="))
        //        {
        //            var p1 = header.IndexOf('=');
        //            var p2 = header.IndexOf(';');
        //            return header.Substring(p1 + 1, p2 - p1 - 1);
        //        }
        //    }
        //    return null;
        //}
    }
}
