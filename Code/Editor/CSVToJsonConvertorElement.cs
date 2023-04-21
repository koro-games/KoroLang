using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace KoroGames.KoroLang.Editor
{

    public class CSVToJsonConvertorElement : VisualElement
    {
        readonly Regex csvParser = new Regex("(?:^|,)(\\\"(?:[^\\\"]+|\\\"\\\")*\\\"|[^,]*)", RegexOptions.Compiled);

        public CSVToJsonConvertorElement() : base()
        {
            var label = new Label();
            label.text = "CSV to JSON Convertor";
            this.Add(label);

            var textProperty = new ObjectField();
            textProperty.objectType = typeof(TextAsset);
            textProperty.allowSceneObjects = false;
            this.Add(textProperty);

            var generateButton = new Button();
            generateButton.text = "Generate";
            generateButton.clicked += () =>
            {
                var text = textProperty.value as TextAsset;
                GenerateJson(text.name, text.text);
                GenerateJsonMany(text.name, text.text);
            };
            this.Add(generateButton);
        }

        public void GenerateJson(string filename, string text)
        {
            var json = ConvertCsvFileToJsonObjectByRows(text);
            filename += ".json";

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

            File.WriteAllText(filename, json);
            AssetDatabase.Refresh();
            Debug.Log("Generate json success");

            //Ping generated file in editor
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(filename));
        }

        public void GenerateJsonMany(string filename, string text)
        {
            var json = ConvertCsvFileToJsonObjectByRows(text);

            var translates = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

            foreach (var translate in translates)
            {
                var translateName = translate["Key"] + ".json";

                var path = "Assets/Resources/Translates/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                translateName = path + translateName;

                if (File.Exists(translateName))
                {
                    Debug.LogError("File already exist");
                    continue;
                }
                File.WriteAllText(translateName, JsonConvert.SerializeObject(translate, Formatting.Indented));
            }
            Debug.Log("Generate json success");
            AssetDatabase.Refresh();
        }


        public string ConvertCsvFileToJsonObjectByRows(string text)
        {
            var lines = text.Split('\n').ToArray();
            string[,] table = new string[ProcessCsvRow(lines[0]).Count(t => true), lines.Length];

            Debug.Log(table.GetLength(0));

            for (int j = 0; j < table.GetLength(1); j++)
            {
                var line = ProcessCsvRow(lines[j]);
                int i = 0;
                foreach (var item in line)
                {
                    table[i++, j] = item;
                }
            }

            //properties equal firs row in table
            var properties = new string[table.GetLength(1)];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = table[0, i];


            var listObjResult = new List<Dictionary<string, string>>();

            for (int i = 0; i < table.GetLength(0); i++)
            {
                var objResult = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                {
                    objResult.TryAdd(properties[j], table[i, j]);
                }
                listObjResult.Add(objResult);
            }

            return JsonConvert.SerializeObject(listObjResult, Formatting.Indented);
        }


        public string ConvertCsvFileToJsonObject(string text)
        {
            var csv = new List<string[]>();
            var lines = text.Split('\n').ToArray();

            foreach (string line in lines)
                csv.Add(line.Split(','));

            var properties = lines[0].Split(',');

            var listObjResult = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                    objResult.Add(properties[j], csv[i][j]);

                listObjResult.Add(objResult);
            }

            return JsonConvert.SerializeObject(listObjResult, Formatting.Indented);
        }

        private IEnumerable<string> ProcessCsvRow(string row)
        {
            MatchCollection results = csvParser.Matches(row);
            foreach (Match match in results)
            {
                foreach (Capture capture in match.Captures)
                {
                    yield return (capture.Value ?? string.Empty).TrimStart(',').Trim('"', ' ');
                }
            }
        }
    }
}

