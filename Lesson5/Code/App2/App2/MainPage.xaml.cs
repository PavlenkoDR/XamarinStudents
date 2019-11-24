using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App2
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public int counterLeft = 0, counterRight = 0;

        public void add(string text, bool right_)
        {
            Frame frame = new Frame
            {
                BorderColor = Color.Aqua
            };
            Label label = new Label();
            label.Text = text;
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

            if (right_)
            {
                swipe.Direction = SwipeDirection.Right;
                swipe.Swiped += (swipeSender, swipeEventArg) =>
                {
                    right.Children.Remove(swipeSender as Frame);
                };
                frame.GestureRecognizers.Add(swipe);
                right.Children.Add(frame);
            }
            else
            {
                swipe.Direction = SwipeDirection.Left;
                swipe.Swiped += async (swipeSender, swipeEventArg) =>
                {
                    if (await DisplayAlert("Confirm the deleting", "Are you sure?", "Yes!", "No"))
                    {
                        left.Children.Remove(frame);
                    }
                };
                frame.GestureRecognizers.Add(swipe);
                left.Children.Add(frame);
            }
        }
        public MainPage()
        {
            InitializeComponent();


            string newName = counterLeft + "Left.txt";
            string newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
            while (File.Exists(newFile))
            {
                string text = File.ReadAllText(newFile);
                add(text, false);
                newName = ++counterLeft + "Left.txt";
                newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
            }
            //counterLeft = Math.Max(0, counterLeft - 1);

            
            newName = counterRight + "Right.txt";
            newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
            while (File.Exists(newFile))
            {
                string text = File.ReadAllText(newFile);
                add(text, true);
                newName = ++counterRight + "Right.txt";
                newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
            }
            //counterRight = Math.Max(0, counterRight - 1);
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

                string newName;
                if (left.Height > right.Height)
                {
                    swipe.Direction = SwipeDirection.Right; 
                    swipe.Swiped += (swipeSender, swipeEventArg) =>
                    {
                        right.Children.Remove(swipeSender as Frame);
                    };
                    frame.GestureRecognizers.Add(swipe);
                    right.Children.Add(frame);

                    newName = counterRight++ + "Right.txt";
                }
                else
                {
                    swipe.Direction = SwipeDirection.Left;
                    swipe.Swiped += async (swipeSender, swipeEventArg) =>
                    {
                        if (await DisplayAlert("Confirm the deleting", "Are you sure?", "Yes!", "No"))
                        {
                            left.Children.Remove(frame);
                        }
                    };
                    frame.GestureRecognizers.Add(swipe);
                    left.Children.Add(frame);

                    newName = counterLeft++ + "Left.txt";
                }

                string newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
                File.WriteAllText(newFile, label.Text);
            };
            Navigation.PushAsync(editor);
        }
    }
}
