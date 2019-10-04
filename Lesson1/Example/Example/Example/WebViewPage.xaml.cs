using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Example
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage()
        {
            InitializeComponent();
            WebView webView = new WebView();
            UrlWebViewSource urlSource = new UrlWebViewSource();
            urlSource.Url = System.IO.Path.Combine("file:///android_asset/", "index.html");
            webView.Source = urlSource;
            Content = webView;
        }

        public interface IBaseUrl { string Get(); }
    }
}