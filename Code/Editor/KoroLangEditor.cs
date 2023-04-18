using UnityEditor;
using UnityEngine;
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
            wnd.titleContent = new GUIContent("KoroLang");
            wnd.minSize = new Vector2(450, 200);
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(new TextSearcherTab(rootVisualElement));
        }
    }
}