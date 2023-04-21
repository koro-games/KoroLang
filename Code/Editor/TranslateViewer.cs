using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KoroGames.KoroLang.Editor
{
    public class TranslateViewer : VisualElement
    {

        private VisualElement _translateRoot;

        public TranslateViewer() : base()
        {
            var label = new Label();
            label.text = "Translate Viewer";
            label.style.borderBottomColor = Color.gray * 0.75f;
            label.style.borderBottomWidth = 2f;
            this.Add(label);

            this.style.flexGrow = 1;
            this.style.flexShrink = 1;

            var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);

            var textLoader = new VisualElement();
            var translateViewer = new VisualElement();
            var scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);

            splitView.Add(textLoader);
            splitView.Add(translateViewer);
            this.Add(splitView);
            translateViewer.Add(scrollView);
            _translateRoot = scrollView;


            var loadButton = new Button();
            loadButton.text = "Load";
            textLoader.Add(loadButton);

            loadButton.clicked += () =>
            {
                textLoader.Clear();
                textLoader.Add(loadButton);
                LoadTranslates(textLoader);
            };
            LoadTranslates(textLoader);
        }

        private void LoadTranslates(VisualElement root)
        {
            var translates = Resources.LoadAll<TextAsset>("Translates/");
            var tree = new ListView();

            tree.fixedItemHeight = 20;
            tree.style.flexGrow = 1;
            tree.selectionType = SelectionType.Single;
            tree.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;

            tree.makeItem = () => CreateTranslateViewer();
            tree.bindItem = (element, i) =>
            {
                var item = (element[0] as ObjectField);
                item.objectType = typeof(TextAsset);
                item.value = translates[i];
                item.style.flexGrow = 1;

                var button = element[1] as Button;
                button.clicked += () =>
                {
                    LoadTranslateToRead(translates[i]);
                };
            };
            tree.itemsSource = translates;
            tree.selectionType = SelectionType.None;
            root.Add(tree);
        }

        private void LoadTranslateToRead(TextAsset textAsset)
        {
            _translateRoot.Clear();
            var translate = JSONToTranslate.JsonConvertToTranslate(textAsset.text);
            var list = new string[translate.TranslateDictionary.Keys.Count];

            //fill list with key and vlaue
            var index = 0;
            foreach (var item in translate.TranslateDictionary)
            {
                list[index++] = $"{item.Key} : {item.Value}";
            }

            var tree = new ListView();

            tree.fixedItemHeight = 20;
            tree.style.flexGrow = 1;
            tree.selectionType = SelectionType.Single;
            tree.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;

            tree.makeItem = () => new Label();
            tree.bindItem = (element, i) =>
            {
                var item = (element as Label);
                item.text = list[i];
            };
            tree.itemsSource = list;
            tree.selectionType = SelectionType.None;
            _translateRoot.Add(tree);

        }


        private VisualElement CreateTranslateViewer()
        {
            var viewer = new VisualElement();
            viewer.style.flexGrow = 1;
            viewer.style.flexShrink = 1;
            viewer.style.flexDirection = FlexDirection.Row;
            viewer.Add(new ObjectField());
            viewer.Add(new Button());
            return viewer;
        }
    }
}
