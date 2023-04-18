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
        private ListView LeftPanel;
        [SerializeField] private Sprite _textIcon;
        [SerializeField] private Sprite _tmpIcon;


        public TextSearcherTab(VisualElement root) : base()
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




            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            root.Add(splitView);

            LeftPanel = new ListView();
            RightPanel = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            splitView.Add(LeftPanel);
            splitView.Add(RightPanel);


            LeftPanel.makeItem = () => TextElementCreator();
            LeftPanel.bindItem = (item, index) =>
            {
                (item as TextElement).Text.text = allObjects[index].name;
                (item as TextElement).Value.text = (allObjects[index] as TMPro.TMP_Text)?.text ?? (allObjects[index] as UnityEngine.UI.Text).text;
                (item as TextElement).Icon.sprite =  (allObjects[index] as TMPro.TMP_Text) != null ? _tmpIcon : _textIcon;
            };

            LeftPanel.itemsSource = allObjects;


            LeftPanel.onSelectionChange += OnSpriteSelectionChange;
            LeftPanel.selectedIndex = m_SelectedIndex;
            LeftPanel.onSelectionChange += (items) => { m_SelectedIndex = LeftPanel.selectedIndex; };
            root.Add(new Label("KoroLang Text Searcher"));
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

                this.style.borderBottomWidth = 1f;
                this.style.borderBottomColor = Color.black;
            }
        }
    }
}
