using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Example
{
    public class SuperClass
    {
        public string FirstField { get; set; }
        public string SecondField { get; set; }
        public int FontSize { get; set; }
        public ImageSource Image { get; set; }
    }

    public class ImageResource : IMarkupExtension
    {
        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            var imageSource = ImageSource.FromResource(Source);

            return imageSource;
        }
    }
    public class EntryValidation : TriggerAction<Entry>
    {
        protected override void Invoke(Entry sender)
        {
            if (!int.TryParse(sender.Text, out var number))
            {
                sender.BackgroundColor = Color.Red;
            }
            else
            {
                sender.BackgroundColor = Color.White;
            }
        }
    }

    public class CustomCell : ViewCell
    {
        Label Text, Description;
        Image image;
    }

    [DesignTimeVisible(false)]
    public partial class MainPage : CarouselPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
