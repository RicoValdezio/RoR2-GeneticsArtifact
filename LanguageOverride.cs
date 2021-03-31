using RoR2;
using System.Collections.Generic;

namespace GeneticsArtifact
{
    class LanguageOverride
    {
        public static Dictionary<string, string> customLanguage;
        internal static void Init()
        {
            customLanguage = new Dictionary<string, string>();

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
            On.RoR2.Language.TokenIsRegistered += Language_TokenIsRegistered;
        }

        private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            if (customLanguage.ContainsKey(token))
            {
                return customLanguage[token];
            }
            return orig(self, token);
        }

        private static bool Language_TokenIsRegistered(On.RoR2.Language.orig_TokenIsRegistered orig, Language self, string token)
        {
            if (customLanguage.ContainsKey(token))
            {
                return true;
            }
            return orig(self, token);
        }
    }
}
