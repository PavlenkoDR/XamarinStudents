using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Example
{
    public class CustomMarkupExtention : IMarkupExtension
    {
        public string first { get; set; }
        public string second { get; set; }
        public string third { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return new [] { first, second, third };
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarkupExtentions : ContentPage
    {
        public const string text = "Test";
        public static double staticDouble = 28.0;
        public MarkupExtentions()
        {
            InitializeComponent();
            listttt.ItemsSource = new string[] { "2222222", "3" };
        }
    }
}