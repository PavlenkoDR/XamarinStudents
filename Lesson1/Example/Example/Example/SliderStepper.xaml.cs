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
    public partial class SliderStepper : ContentPage
    {
        public SliderStepper()
        {
            InitializeComponent();
        }

        private void ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double ttt = 123.0;
            int aaa = 3;
            string dfdf = "ddssd";
            label.Text = $"Selected {e.NewValue:F1}";
        }
    }
}