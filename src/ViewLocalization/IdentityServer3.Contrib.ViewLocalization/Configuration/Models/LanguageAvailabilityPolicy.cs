using System;

namespace IdentityServer3.Contrib.ViewLocalization.Configuration
{
    public class LanguageAvailabilityPolicy
    {
        private LanguageAvailabilityPolicy()
        {
            throw new NotImplementedException();
        }
        
        public static LanguageAvailabilityPolicy AllExcept(bool allowRequestNegotiation, params string[] exceptIsoLanguagesNames)
        {
            return new LanguageAvailabilityPolicy();
        }

        public static LanguageAvailabilityPolicy Only(bool allowRequestNegotiation, params string[] onlyIsoLanguagesNames)
        {
            return new LanguageAvailabilityPolicy();
        }
    }
}