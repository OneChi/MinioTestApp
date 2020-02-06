using MinioTApp2.Model.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MinioTApp2.Resources.TemplateSelector
{
    class ViewPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BucketTemplate { get; set; }

        public DataTemplate ItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var newItem = item as MinioItemModel;
            if (newItem != null)
            {
                return ItemTemplate;
            }
            return BucketTemplate;
        }


    }
}
