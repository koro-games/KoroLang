using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace KoroGames.KoroLang
{
    public static class JSONToTranslate
    {
        //Json to translate convert
        public static Translate JsonConvertToTranslate(string text)
        {
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            return new Translate(dictionary);
        }
    }
}
