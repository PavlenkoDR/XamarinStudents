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
            editor.Disappearing += (s, _e) => {
                if (editor.text == null) return;
                Frame frame = new Frame
                {
                    BorderColor = Color.Aqua
                 };
                Label label = new Label();
                label.Text = editor.text;  
                label.BackgroundColor = Color.Beige;
                frame.Content = label;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (tapsender, tape) =>
                {
                    EditorPage editor2 = new EditorPage(label.Text);
                    editor2.Disappearing += (a, b) =>
                    {
                        label.Text = editor2.text;
                    };
                    Navigation.PushAsync(editor2);
                };
                label.GestureRecognizers.Add(tapGestureRecognizer);
                var swipe = new SwipeGestureRecognizer();
                

                if (left.Height > right.Height)
                { 
                    right.Children.Add(frame);
                    swipe.Direction = SwipeDirection.Right;
                    swipe.Swiped += (sender_, e_) =>
                    {
                        right.Children.Remove(frame);
                        
                    };
                }
                else
                {
                    left.Children.Add(frame);
                    swipe.Direction = SwipeDirection.Left;
                    swipe.Swiped += (sender_, e_) =>
                    {
                        left.Children.Remove(frame);
                        
                    };
                }
            };
            Navigation.PushAsync(editor);

        }
    }
}
