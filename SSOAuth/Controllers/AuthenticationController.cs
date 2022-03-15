using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SSOAuth.Business.Interface;
using SSOAuth.Cache;
using SSOAuth.Data.Entity.Authorization;
using SSOAuth.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SSOAuth.Controllers
{
    //[Route("api/Authentication")]
    //[ApiController]
    public class AuthenticationController : Controller
    {






        // API - 3
        // /oauth/authorizationcode

        private readonly IAuthService _authService;
        private readonly ICachingService _cacheServ;

        public AuthenticationController(IAuthService authService, ICachingService cacheServ)
        {
            this._authService = authService;
            this._cacheServ = cacheServ;
        }


        // API - 1
        //https://domain.com/o/oauth2/v1/auth (get method)
        //this should be the URL to get code
        // data it should have is :
        //client_id, scope, redirect_uri (this should be exact same as provider's domain entered in DB),
        // state (this is the redirect URL where this system should redirect to after authentication)
        //
        [HttpGet]
        [AllowAnonymous]
        [Route("/o/oauth2/v1/auth")]
        public async Task<IActionResult> auth(string client_id, string scope, string redirect_uri,string state)
        {
            
            bool isScopeVerified = false;            
            Provider provider = new Provider();

            provider = await _authService.GetAuthClientId(client_id);
            if (provider != null)
            {
                isScopeVerified = await _authService.VerifyAuthScope(scope);
                if (isScopeVerified)
                {                    
                    if (!string.IsNullOrEmpty(provider.Domain) && provider.Domain.ToLower() == redirect_uri.ToLower())
                    {
                        // check if already logged in 
                        var principle = HttpContext.User as ClaimsPrincipal;
                        if (principle.GetIdentityId() != null)
                        {
                            var identityGuid = Guid.Parse(principle.GetIdentityId());
                            var token = await _cacheServ.GetTokenByIdentity(identityGuid);
                            if (token != null)
                            {
                                var code = await _authService.AuthorizationCodeGeneration(provider, token.Token, identityGuid);
                                //https://provider.domain.com/auth?code=4/P7q7W91a-oMsCeLvIaQm6bTrgtp7&Redirect={clientPage}
                                // also see url of /signin-oidc
                                //redirect to domain

                                return Redirect(_authService.GetCallbackPath(provider.Domain, code, state));
                            }
                            else
                            {
                                // in case user have cookies and our cache doesn't have data , remove their cookie and make 
                                // them sign in again
                                await HttpContext.SignOutAsync(
CookieAuthenticationDefaults.AuthenticationScheme);
                                return RedirectToAction("Login", "Account", new { redirectUrl = state, clientId = client_id });
                            }
                        }
                        return RedirectToAction("Login", "Account", new { redirectUrl = state, clientId = client_id });
                    }
                    else
                    {
                        //Invalid Domain Exception
                        throw new HttpRequestException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            ReasonPhrase = "Bad Request",
                            Content = new StringContent("Exception: Invalid Domain"),
                        }.ToString());
                    }
                }
                else
                {
                    //Invalid Scope Exception
                    throw new HttpRequestException (new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = "Bad Request",
                        Content = new StringContent("Exception: Invalid Scope"),
                    }.ToString());                    
                }
            }
            else
            {
                //Client Not Valid Exception
                throw new HttpRequestException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Bad Request",
                    Content = new StringContent("Exception: Invalid Client Id"),
                }.ToString());
            }                                               
        }


        // API - 2
        // https://domain.com/oauth2/v1/token (post method)
        // this will be done by client's server
        // and it will containt code that was provided by API-1
        // and other stuff client_id,client_secret,redirect_uri , grant_type
        // on verification of all this will return token (that was earlier given to user)
        // response : access_token,expires_in,id_token,scope,token_type (this will be bearer),refresh_token (optional)
        // 
        [HttpPost]
        [Route("/oauth2/v1/token")]
        public async Task<AccessTokenResponseDTO> Post([FromForm] TokenRequestDTO data)
        {
            // this will check current system request source URL, get provider against client id and client secret
            // match that provider domain with above source URL if it doesn't match return invalid client
            // grant_type should be equal to 'authorization_code' that;s what we are implementing for these systems now
            // and finally check code if that exists in cache return token in case all case 


            // error types 
            // invalid_request = request is missing a parameter
            // invalid_client = invalid client ID or secret
            // invalid_grant = invalid grant or if redirect uri != URL provided in request
            // unauthorized_client = if client isn't allowed on this grant type ( this will never occur in this system as all
            //                       are same grant type
            // unsupported_grant_type = any other grant than "authorization_code"
            ///
            var validationResp = ValidateForTokenReques(data);
            if (!string.IsNullOrEmpty(validationResp.Error))
            {
                return validationResp;
            }

            var httpRequestFeature = HttpContext.Request.HttpContext.Features.Get<IHttpRequestFeature>();
            var providerToken = await _cacheServ.GetProviderTokenByCode(data.Code);
            if (providerToken == null)
            {
                return new AccessTokenResponseDTO() { Error = "Cannot find logged in client", ErrorDescription = "Either code is incorrect or oauth server just restarted" };
            }
            else 
            {
                if (data.Domain != providerToken.Domain && data.Domain != "::1") // calling client should be either hosted on domain with entry in DB or
                                                                          // localhost ::1
                {
                    return new AccessTokenResponseDTO() { Error = "Invalid client", ErrorDescription = "Calling URL doesn't match with client Domain " };
                }
                else if (data.Grant_type != "authorization_code")
                {
                    return new AccessTokenResponseDTO() { Error = "Invalid Grant type", ErrorDescription = "Invalid Grant type requested" };
                }
                else 
                {
                    var token = providerToken.Token;
                    if (token != null)
                    {
                        return new AccessTokenResponseDTO()
                        {
                            Access_token = token,
                            Token_type = "Bearer",
                        };
                    }
                }
            }
            return new AccessTokenResponseDTO()
            {

            };
        }

        private AccessTokenResponseDTO ValidateForTokenReques(TokenRequestDTO data)
        {
            if (data == null)
                return new AccessTokenResponseDTO()
                {
                    Error = "No data provided",
                    ErrorDescription = "Please validate your properties"
                };
            else if(string.IsNullOrEmpty(data.Client_id) || string.IsNullOrEmpty(data.Client_secret))
            {
                return new AccessTokenResponseDTO()
                {
                    Error = "invalid_request",
                    ErrorDescription = "Client Id or secret is missing"
                };
            }
            else if (string.IsNullOrEmpty(data.Grant_type))
            {
                return new AccessTokenResponseDTO()
                {
                    Error = "invalid_request",
                    ErrorDescription = "Grant type is missing"
                };
            }
            else if (string.IsNullOrEmpty(data.Code))
            {
                return new AccessTokenResponseDTO()
                {
                    Error = "invalid_request",
                    ErrorDescription = "Code is not provided"
                };
            }
            return new AccessTokenResponseDTO()
            {
                
            };
        }


        // GET: api/<AuthenticationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthenticationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthenticationController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<AuthenticationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthenticationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
