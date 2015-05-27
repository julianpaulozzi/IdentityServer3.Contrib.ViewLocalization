using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using IdentityServer3.Contrib.ViewLocalization;
using IdentityServer3.Contrib.ViewLocalization.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.Client;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using ViewLocalizationSample;
using ViewLocalizationSample.Config;
using AuthenticationOptions = Thinktecture.IdentityServer.Core.Configuration.AuthenticationOptions;

[assembly: OwinStartup(typeof(Startup))]

namespace ViewLocalizationSample
{
    public class Startup
    {
        private const string SelfHostUri = "http://localhost:12080";
        private const string IdsrvPath = "/core";
        private const string AuthorityUri = SelfHostUri + IdsrvPath;
        private const string UserInfoClientUri = AuthorityUri + "/connect/userinfo";
        private const string TokenClientUri = AuthorityUri + "/connect/token";

        public void Configuration(IAppBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());

            app.Map(IdsrvPath, coreApp =>
            {
                var factory = InMemoryFactory.Create(
                    users: Users.Get(),
                    clients: Clients.Get(),
                    scopes: Scopes.Get());

                var options = new IdentityServerOptions
                {
                    IssuerUri = "https://idsrv3.com",
                    SiteName = "IdentityServer3 - LocalizedViewService",
                    RequireSsl = false,
                    EnableWelcomePage = true,
                    SigningCertificate = Certificate.Get(),
                    Factory = factory,
                    CorsPolicy = CorsPolicy.AllowAll,
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureAdditionalIdentityProviders,
                    }
                };

                var viewLocalizationOptions = new IdentityServerViewLocalizationOptions
                {
                    LocalizationServiceOptions =
                    {
                        // Add not standard scopes
                        ScopeLocalizationRequest = info =>
                        {
                            return new ScopeLocalizationCollection
                            {
                                {
                                    "pt-BR", new ScopeTranslations
                                    {
                                        {"read", "Ler dados", "Ler dados do aplicativo"},
                                        // {"write", "Escrever dados" } // Fallback use default
                                    }
                                },
                                // override defaults
                                {
                                    "en", new ScopeTranslations
                                    {
                                        {"read", "Read application data"},
                                        {"write", "Write application data", "Custom description of Write data"}
                                    }
                                }
                            };
                        }
                    }
                };


                // This "Use" order is important
                coreApp.UseIdentityServerViewLocalization(options, viewLocalizationOptions);
                coreApp.UseIdentityServer(options);
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "owin",
                Authority = AuthorityUri,
                RedirectUri = SelfHostUri,
                PostLogoutRedirectUri = SelfHostUri,
                SignInAsAuthenticationType = "Cookies",
                ResponseType = "code id_token token",
                Scope = "openid profile email offline_access phone roles all_claims address",
                // Scope = "openid profile offline_access read write",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        // filter "protocol" claims
                        var claims = new List<Claim>(from c in n.AuthenticationTicket.Identity.Claims
                                                     where c.Type != "iss" &&
                                                           c.Type != "aud" &&
                                                           c.Type != "nbf" &&
                                                           c.Type != "exp" &&
                                                           c.Type != "iat" &&
                                                           c.Type != "nonce" &&
                                                           c.Type != "c_hash" &&
                                                           c.Type != "at_hash"
                                                     select c);

                        // get userinfo data
                        var userInfoClient = new UserInfoClient(
                            new Uri(UserInfoClientUri),
                            n.ProtocolMessage.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();
                        userInfo.Claims.ToList().ForEach(ui => claims.Add(new Claim(ui.Item1, ui.Item2)));

                        // get access and refresh token
                        var tokenClient = new OAuth2Client(
                            new Uri(TokenClientUri),
                            "owin",
                            "secret");

                        var response = await tokenClient.RequestAuthorizationCodeAsync(n.Code, n.RedirectUri);

                        claims.Add(new Claim("access_token", response.AccessToken));
                        claims.Add(new Claim("expires_at", DateTime.Now.AddSeconds(response.ExpiresIn).ToLocalTime().ToString()));
                        claims.Add(new Claim("refresh_token", response.RefreshToken));
                        claims.Add(new Claim("id_token", n.ProtocolMessage.IdToken));

                        n.AuthenticationTicket = new AuthenticationTicket(new ClaimsIdentity(claims.Distinct(new ClaimComparer()), n.AuthenticationTicket.Identity.AuthenticationType), n.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = async n =>
                    {
                        // if signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token").Value;
                            n.ProtocolMessage.IdTokenHint = idTokenHint;
                        }
                    }
                }

            });

            var httpConfig = new HttpConfiguration();

            httpConfig.MapHttpAttributeRoutes();
            httpConfig.Routes.MapHttpRoute("Home", "", new { controller = "Page", action = "Index" });

            httpConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(httpConfig);
        }

        public static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "767400843187-8boio83mb57ruogr9af9ut09fkg56b27.apps.googleusercontent.com",
                ClientSecret = "5fWcBT0udKY7_b6E3gEiJlze"
            };
            app.UseGoogleAuthentication(google);

            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "676607329068058",
                AppSecret = "9d6ab75f921942e61fb43a9b1fc25c63"
            };
            app.UseFacebookAuthentication(fb);

            var twitter = new TwitterAuthenticationOptions
            {
                AuthenticationType = "Twitter",
                SignInAsAuthenticationType = signInAsType,
                ConsumerKey = "N8r8w7PIepwtZZwtH066kMlmq",
                ConsumerSecret = "df15L2x6kNI50E4PYcHS0ImBQlcGIt6huET8gQN41VFpUCwNjM"
            };
            app.UseTwitterAuthentication(twitter);
        }
    }
}
