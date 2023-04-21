using UnityEngine.UIElements;

namespace KoroGames.KoroLang.Editor
{
    public class TranslateCreator : VisualElement
    {
        public TranslateCreator() : base()
        {
            this.style.flexGrow = 1;
            this.style.flexShrink = 1;

            var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);
            this.Add(splitView);
            splitView.Add(new CSVToJsonConvertorElement());
            splitView.Add(new TranslateViewer());
        }
    }
}
