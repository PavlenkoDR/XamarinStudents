using Android.Util;
using Android.Widget;
using Example;
using Example.Droid;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HeaderView), typeof(HeaderViewRenderer))]
[assembly: Dependency(typeof(AlertClassDroid))]
namespace Example.Droid
{
    public class AlertClassDroid : IAlertClass
    {
        public void ShowAlert()
        {
            Console.WriteLine("Android value");
        }
    }


    public class HeaderViewRenderer : ViewRenderer<HeaderView, TextView>
    {
        TextView textView;
        public HeaderViewRenderer(Android.Content.Context context) : base(context)
        {
            textView = new TextView(Context);
            textView.SetTextSize(ComplexUnitType.Dip, 28);
            LongClick += (sender, args) =>
            {
                Element.HandleLongPress(this, EventArgs.Empty);
                Toast.MakeText(this.Context, "Long press", ToastLength.Short).Show();
            };
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HeaderView> args)
        {
            base.OnElementChanged(args);
            if (Control == null)
            {
                SetNativeControl(textView);
            }
            if (args?.NewElement != null)
            {
                SetText();
                SetTextColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            SetNativeControl(textView);
            if (e?.PropertyName == HeaderView.TextColorProperty.PropertyName)
            {
                SetTextColor();
            }
            else if (e?.PropertyName == HeaderView.TextProperty.PropertyName)
            {
                SetText();
            }
        }
        private void SetText()
        {
            Control.Text = Element.Text;
        }
        private void SetTextColor()
        {
            Android.Graphics.Color andrColor = Android.Graphics.Color.Gray;

            if (Element.TextColor != Color.Default)
            {
                Color color = Element.TextColor;
                andrColor = Android.Graphics.Color.Argb(
                    (byte)(color.A * 255),
                    (byte)(color.R * 255),
                    (byte)(color.G * 255),
                    (byte)(color.B * 255));
            }
            Control.SetTextColor(andrColor);
        }
    }
}