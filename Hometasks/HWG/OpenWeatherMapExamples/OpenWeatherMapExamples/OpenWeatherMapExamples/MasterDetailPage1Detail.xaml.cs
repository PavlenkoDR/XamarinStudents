using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OpenWeatherMapExamples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1Detail : ContentPage
    {
        IWebApi _webApi;
        
        public MasterDetailPage1Detail()
        {
            InitializeComponent();
            Init();
        }
        public async void Init()
        {
            _webApi = new WebApi();
            var forecast = await _webApi.GetForecast();
            BindableLayout.SetItemsSource(stackForecast, forecast);
        }
        
        public MasterDetailPage1Detail(string labelText) : this()
        {
            label.Text = labelText;
        }
    }
}