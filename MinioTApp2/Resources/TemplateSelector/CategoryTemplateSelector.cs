using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MinioTApp2.Resources.TemplateSelector
{

    public class CategoryBase { }

    public class Category : CategoryBase
    {
        public string Name { get; set; }
        public Symbol Glyph { get; set; }
    }

    public class Separator : CategoryBase { }

    public class Header : CategoryBase
    {
        public string Name { get; set; }
    }

    class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; private set; }
        public DataTemplate HeaderTemplate { get; private set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is Separator ? SeparatorTemplate : item is Header ? HeaderTemplate : ItemTemplate;
        }
    }

}
