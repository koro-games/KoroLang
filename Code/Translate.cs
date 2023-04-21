using System.Collections.Generic;

namespace KoroGames.KoroLang
{
    public class Translate
    {
        public string TranslateCode;
        public Dictionary<string, string> TranslateDictionary;

        public Translate(Dictionary<string, string> dictionary)
        {
            TranslateDictionary = dictionary;
        }

        public string GetTranslate(string key) => TranslateDictionary.TryGetValue(key, out var result) ? result : key;
    }
}
