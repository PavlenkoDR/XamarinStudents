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
    public partial class MessagingCenterPage : ContentPage
    {
        public MessagingCenterPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<Page, string>(this, "ValidationMessage", entry.Text);
        }
    }
}