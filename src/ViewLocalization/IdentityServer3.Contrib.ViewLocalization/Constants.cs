namespace IdentityServer3.Contrib.ViewLocalization
{
    public class VLConstants
    {
        public static class Localization
        {
            public const string DefaultFallbackLanguage = "en";
            internal static readonly LanguagesCollection AvailableLanguages = new LanguagesCollection
            {
                { "en" },
                { "pt-BR" },
                { "fr" }
            };
        }

        public static class LocalizationParts
        {
            public const string Commun_Part = "commun";
            public const string Welcome_Part = "welcome";
            public const string Login_Part = "login";
            public const string Logout_Part = "logout";
            public const string Loggedout_Part = "loggedout";
            public const string Consent_Part = "consent";
            public const string Permissions_Part = "permissions";
            public const string Error_Part = "error";
            public const string Scopes_Part = "scopes";
        }

        public static class Cookies
        {
            public const string LanguageCookieName = "NG_TRANSLATE_LANG_KEY";
        }

        public static class RouteNames
        {
            public const string Welcome = "locale.welcome";
        }

        public static class RoutePaths
        {
            public const string Locale = "locale";
            public const string LocaleWithPart = "locale/{part}";
            public const string Scripts = "scripts/{action}";
        }

        public static class Controllers
        {
            public const string Welcome = "Welcome";
            public const string Localization = "Localization";
        }
    }
}