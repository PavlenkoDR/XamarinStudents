using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Example
{
    partial class DynamicLoadFromXaml : ContentPage
    {
        public DynamicLoadFromXaml()
        {
            string pageXAML = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
            "<ContentPage xmlns=\"http://xamarin.com/schemas/2014/forms\"\n" +
            "xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"\n" +
            "x:Class=\"HelloApp.MainPage\"\n" +
            "Title=\"Main Page\">\n" +
            "<Label Text=\"Example\" HorizontalOptions=\"Center\" VerticalOptions=\"CenterAndExpand\" />" +
            "</ContentPage>";

            this.LoadFromXaml(pageXAML);
        }
    }
}
