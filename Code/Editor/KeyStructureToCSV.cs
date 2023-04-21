using UnityEditor;
using UnityEngine;
using System.IO;


namespace KoroGames.KoroLang.Editor
{
    public static class KeyStructureToCSV
    {
        public static void SaveKeyStructure(string[] keys)
        {
            var filename = "KeyForExport.csv";


            var sb = new System.Text.StringBuilder("Key" + ",lang\n");
            for (int i = 0; i < keys.Length; i++)
            {
                sb.Append(keys[i] + ",value\n");
            }



            //add file path to resuorses in assets
            var path = "Assets/Resources/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            filename = path + filename;

            //check if file already exist
            if (File.Exists(filename))
            {
                Debug.LogError("File already exist");
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(filename));
                return;
            }

            File.WriteAllText(filename, sb.ToString());
            AssetDatabase.Refresh();
            Debug.Log("Generate json success");

            //Ping generated file in editor
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(filename));
        }
    }
}

