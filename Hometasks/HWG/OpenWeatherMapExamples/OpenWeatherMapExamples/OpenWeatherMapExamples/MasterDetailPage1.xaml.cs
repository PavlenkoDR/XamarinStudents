using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OpenWeatherMapExamples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1 : MasterDetailPage
    {
        public MasterDetailPage1()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        public void SelectedPage(string pageName, string labelText = "THIS")
        {
            Page page = null;
            if (pageName == "Search")
            {
                page = new MainPage();
            }
            else if (pageName == "Main")
            {
                page = new MasterDetailPage1Detail(labelText);
            }
            if (page != null)
            {
                page.Title = pageName;

                Detail = new NavigationPage(page);
                IsPresented = false;
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterDetailPage1MasterMenuItem;
            if (item == null)
                return;
            SelectedPage(item.Title);
            MasterPage.ListView.SelectedItem = null;
        }
    }
}