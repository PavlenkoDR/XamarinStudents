using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App2
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            EditorPage editor = new EditorPage();
            editor.Disappearing += (s,_e) => {
                if (editor.text == null) return;

                Label label = new Label();
                label.Text = editor.text;
                                
                label.BackgroundColor = Color.Red;

                data.Children.Add(label);
            };
            Navigation.PushAsync(editor);

        }
    }
}
