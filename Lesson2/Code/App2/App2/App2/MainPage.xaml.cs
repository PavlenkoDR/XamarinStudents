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
        string GetHeader(string s) {
            s = s.Split(new[] { "\n" }, StringSplitOptions.None)[0];
            s = $"{s.Substring(0, Math.Min(50, s.Length))} ...";
            return s;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            EditorPage editor = new EditorPage();
            editor.Disappearing += (s,_e) => {
                if (editor.text == null) return;

                Label label = new Label();
                var text = editor.text;
                label.Text = GetHeader(text);
                                
                label.BackgroundColor = Color.Red;

                // Обработка жеста (tapped => клик)
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += (_s, __e) =>
                {
                    EditorPage _editor = new EditorPage(text);
                    _editor.Disappearing += (__s, ___e) =>
                    {
                        text = _editor.text;
                        label.Text = GetHeader(text);
                    };
                    Navigation.PushAsync(_editor);
                };
                label.GestureRecognizers.Add(tap);
                //

                data.Children.Add(label);
            };
            Navigation.PushAsync(editor);

        }
    }
}
