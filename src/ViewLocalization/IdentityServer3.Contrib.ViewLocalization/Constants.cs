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
            };
        }
        
        public static class RouteNames
        {
            public const string Welcome = "locale.welcome";
        }

        public static class Controllers
        {
            public const string Welcome = "Welcome";
            public const string Localization = "Localization";
        }
    }
}