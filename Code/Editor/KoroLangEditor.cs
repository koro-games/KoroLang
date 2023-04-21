using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using System;
using System.Reflection;

namespace KoroGames.KoroLang.Editor
{
    public partial class MyCustomEditor : EditorWindow
    {
        [MenuItem("Tools/KoroTools/KoroLang")]
        public static void ShowMyEditor()
        {
            EditorWindow wnd = GetWindow<MyCustomEditor>();

            var texture2D = Resources.Load<Texture2D>("Icon");

            wnd.titleContent = new GUIContent("KoroLang", texture2D);
            wnd.minSize = new Vector2(450, 200);
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(new TabMenu(rootVisualElement));


        }
    }
}
//"$(ProjectPath)" -g "$(File)":$(Line):$(Column)