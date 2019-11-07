using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Example
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            int a = 24;

            Func<string> bar = () => {
                a++;
                return "123";
            };
            var fff = bar();

            MessagingCenter.Subscribe<Page, string>(this, "ValidationMessage", (_sender, arg) =>
            {
                _sender.Title = "222222";
                DisplayAlert("Message received", arg, "OK");
            });
        }

        void foo<TK>(TK value)
        {

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DynamicLoadFromXaml());
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MarkupExtentions());
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PickerPage());
        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SliderStepper());
        }

        private void Button_Clicked_4(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WebViewPage());
        }

        private void Button_Clicked_5(object sender, EventArgs e)
        {
        }

        private void Button_Clicked_6(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MessagingCenterPage());
        }

        private void Button_Clicked_7(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ImageDrawablePage());
        }

        private void Button_Clicked_8(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ImageEntryPage());
        }
    }
}
