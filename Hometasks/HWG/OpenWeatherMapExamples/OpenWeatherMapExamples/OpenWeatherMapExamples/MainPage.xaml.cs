using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OpenWeatherMapExamples
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var mainPage = Application.Current.MainPage as MasterDetailPage1;
            if (mainPage != null)
            {
                activityIndicator.IsRunning = true;
                Task.Run(() =>
                {
                    CityLoader.cityListTask.Wait();
                    while (CityLoader.cityList == null) {}
                    var findedCities = CityLoader.cityList.Where(x => x.name.IndexOf(e.NewTextValue) == 0).Select(x => x.name).ToList();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        CityListView.ItemsSource = findedCities;
                        activityIndicator.IsRunning = false;
                    });
                });
            }
        }

        private void CityListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var mainPage = Application.Current.MainPage as MasterDetailPage1;
            if (mainPage != null)
            {
                mainPage.SelectedPage("Main", e.SelectedItem.ToString());
            }
        }
    }
}
