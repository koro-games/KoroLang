using UnityEngine;
using UnityEngine.UIElements;

namespace KoroGames.KoroLang.Editor
{
    public class TabMenu : VisualElement
    {
        private VisualElement[] _tabsContent;
        private VisualElement _rootMenu;
        private int _id;

        private void Init()
        {
            _tabsContent = new VisualElement[]{
                 new TranslateCreator(),
                 new TextSearcherTab(),
                 new Label("Huh?"),
            };
        }

        private void UpdateTab(int id)
        {
            _rootMenu.Clear();
            _rootMenu.Add(_tabsContent[id]);
            _tabsContent[id].StretchToParentSize();
        }

        public TabMenu(VisualElement root) : base()
        {
            var tabMenu = new VisualElement();
            root.Add(tabMenu);
            var content = new VisualElement();
            root.Add(content);
            _rootMenu = content;
            _rootMenu.style.flexGrow = 1;
            _rootMenu.style.flexShrink = 1;
            Init();
            _rootMenu.Add(new TextSearcherTab());

            tabMenu.style.flexDirection = FlexDirection.Row;
            tabMenu.style.borderBottomColor = Color.gray * 0.75f;
            tabMenu.style.borderBottomWidth = 2;
            for (int i = 0; i < _tabsContent.Length; i++)
            {
                var id = i;
                var tabElement = new Button(() => UpdateTab(id)) { text = _tabsContent[i].GetType().Name };
                tabMenu.Add(tabElement);
            }
            UpdateTab(0);
        }
    }
}
//"$(ProjectPath)" -g "$(File)":$(Line):$(Column)