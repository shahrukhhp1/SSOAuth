# SSOAuth
Single Sign On Authentication system based on OAuth2 protocols , currently authorization code flow is implemented.





How to Connect to SSO Auth Server (OAuth2 protocol)

In middleware of current system, check for cookies there is no bearer token, proceed to following:
1 – STEP 1
 Redirect to  “{ssoAuthDoman} /o/oauth2/v1/auth” with params :
client_id
scope (all)
redirect_uri (this should be exact same as provider's domain entered in DB)
state (this is the redirect URL where this system should redirect to after authentication)

2- STEP 2
 You will get callback at your application on /auth URL , like :
[GET]
www.yoursite.com/singin-oidc?code={authorizationCode}&redirect={page_you_sent_from}

3 – STEP 3
 You should then call following URL to get token :
[Post]
{ssoAuthDoman}/oauth2/v1/token
With object:
{
 Client_id, Client_secret, Code {that was sent by ssoAuth},
 Grant_type {authorization_code always},
 Redirect_uri {redirect URL}, 
 Domain {base URL of client}
}

In response to this you will get :
{
                            Access_token,
                            Token_type = "Bearer"
}
This shall be saved for current user.
