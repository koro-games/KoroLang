using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace KoroGames.KoroLang.Editor
{
    public class TextSearcherTab : VisualElement
    {

        [SerializeField] private int m_SelectedIndex = -1;
        private VisualElement RightPanel;
        private ListView listView;
        [SerializeField] private Sprite _textIcon;
        [SerializeField] private Sprite _tmpIcon;


        public TextSearcherTab() : base()
        {

            var texture2D = ToTexture2D(EditorGUIUtility.ObjectContent(null, typeof(UnityEngine.UI.Text)).image);
            _textIcon = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));

            //Load sprite from resources
            texture2D = Resources.Load<Texture2D>("TMPIcon");
            _tmpIcon = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));


            var tmObjects = SceneAsset.FindObjectsOfType<TMPro.TMP_Text>();
            var textObjects = SceneAsset.FindObjectsOfType<UnityEngine.UI.Text>();

            //merge tmObjects and textObjects in one array as Object[]
            var allObjects = new List<Object>();
            allObjects.AddRange(tmObjects);
            allObjects.AddRange(textObjects);
            //remove duplicates
            allObjects = allObjects.Distinct().ToList();
            //sort by name
            allObjects = allObjects.OrderBy(o => o.name).ToList();

            this.style.flexGrow = 1;
            this.style.flexShrink = 1;

            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            this.Add(splitView);

            var LeftPanel = new VisualElement();
            var RightPanel = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            splitView.Add(LeftPanel);
            splitView.Add(RightPanel);
            listView = new ListView();
            LeftPanel.Add(listView);

            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            listView.makeItem = () => TextElementCreator();
            listView.bindItem = (item, index) =>
            {
                (item as TextElement).Text.text = allObjects[index].name;
                (item as TextElement).Value.text = (allObjects[index] as TMPro.TMP_Text)?.text ?? (allObjects[index] as UnityEngine.UI.Text).text;
                (item as TextElement).Icon.sprite = (allObjects[index] as TMPro.TMP_Text) != null ? _tmpIcon : _textIcon;
            };

            listView.itemsSource = allObjects;


            listView.onSelectionChange += OnSpriteSelectionChange;
            listView.selectedIndex = m_SelectedIndex;
            listView.onSelectionChange += (items) => { m_SelectedIndex = listView.selectedIndex; };

            var exportButton = new Button();
            exportButton.text = "Export";
            exportButton.clicked += () =>
            {
                var keys = new List<string>(allObjects.Count);

                for (int i = 0; i < allObjects.Count; i++)
                {
                    var text = (allObjects[i] as TMPro.TMP_Text)?.text ?? (allObjects[i] as UnityEngine.UI.Text).text;
                    text = text.Trim();
                    if (!keys.Contains(text))
                        keys.Add(text);
                }
                KeyStructureToCSV.SaveKeyStructure(keys.ToArray());
            };
            LeftPanel.Add(exportButton);


            var textFixButton = new Button();
            textFixButton.text = "Text Fix";
            textFixButton.clicked += () =>
            {
                foreach (var item in allObjects)
                {
                    if (item is TMPro.TMP_Text)
                    {
                        var text = item as TMPro.TMP_Text;
                        text.text = text.text.ToUpper();
                        text.text = text.text.Trim();
                    }
                    else
                    {
                        var text = item as UnityEngine.UI.Text;
                        //make text uppercase
                        text.text = text.text.ToUpper();
                        text.text = text.text.Trim();
                    }
                }
            };
            LeftPanel.Add(textFixButton);
        }

        private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
        {
            RightPanel.Clear();
            Object item = selectedItems.First() as Object;
            SerializedObject serializedObject = null;
            SerializedProperty serializedProperty = null;

            if (item is TMPro.TMP_Text)
            {
                serializedObject = new SerializedObject(item);
                serializedProperty = serializedObject.FindProperty("m_text");
            }
            else
            {
                item = selectedItems.First() as UnityEngine.UI.Text;
                if (item == null) return;

                serializedObject = new SerializedObject(item);
                serializedProperty = serializedObject.FindProperty("m_Text");
            }

            var property = new PropertyField(serializedProperty);

            property.Bind(serializedObject);
            property.RegisterValueChangeCallback(evt =>
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            });
            RightPanel.Add(property);
        }


        public Texture2D ToTexture2D(Texture texture)
        {
            return Texture2D.CreateExternalTexture(
                texture.width,
                texture.height,
                TextureFormat.RGB24,
                false, false,
                texture.GetNativeTexturePtr());
        }

        private VisualElement TextElementCreator()
        {
            var textElement = new TextElement();
            textElement.Text.text = "Hello World!";
            textElement.Value.text = "Hello World!";
            return textElement;
        }

        public class TextElement : VisualElement
        {

            public Image Icon;
            public Label Text;
            public Label Value;
            public TextElement() : base()
            {
                this.style.flexDirection = FlexDirection.Row;

                Icon = new Image();
                Icon.scaleMode = ScaleMode.ScaleToFit;
                Icon.style.width = 25;
                Icon.style.height = 25;

                this.Add(Icon);

                var textSide = new VisualElement();
                this.Add(textSide);

                Text = new Label();
                Text.style.fontSize = 12;
                textSide.Add(Text);

                Value = new Label();
                Value.style.fontSize = 12;
                Value.style.color = Color.gray;
                textSide.Add(Value);
            }
        }
    }
}
