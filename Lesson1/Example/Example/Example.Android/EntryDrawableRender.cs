using System.Collections.Generic;
using Android.Content;
using Android.Text;
using Android.Text.Style;
using Example.Droid;
using Example.Controls;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EntryDrawable), typeof(EntryDrawableRender))]
namespace Example.Droid
{
    public class EntryDrawableRender : EntryRenderer
    {
        private readonly Dictionary<string, int> Emoticons;

        public EntryDrawableRender(Context context) : base(context)
        {
            Emoticons = new Dictionary<string, int>
            {
                {":-)", Resource.Drawable.logo},
                {":)", Resource.Drawable.icon}
            };
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e?.OldElement != null || e.NewElement == null)
                return;

            //var imageSpan = new ImageSpan(this, Resource.Drawable.icon); //Find your drawable.
            //var spannableString = new SpannableString(textView.Text); //Set text of SpannableString from TextView
            //spannableString.SetSpan(imageSpan, textView.Text.Length - 1, textView.Text.Length, 0); //Add image at end of string

            //textView.TextFormatted = spannableString; //Assign string to TextView (Use TextFormatted for Spannables)

            //_editText = FindViewById<EditText>(Resource.Id.editText1);
            //_editText.AfterTextChanged += (sender, args) => SpannableTools.AddSmiles(this, args.Editable);
        }

        public bool AddSmiles(Context context, ISpannable spannable)
        {
            var hasChanges = false;
            foreach (var entry in Emoticons)
            {
                var smiley = entry.Key;
                var smileyImage = entry.Value;
                var indices = spannable.ToString().IndexesOf(smiley);
                foreach (var index in indices)
                {
                    var set = true;
                    foreach (ImageSpan span in spannable.GetSpans(index, index + smiley.Length, Java.Lang.Class.FromType(typeof(ImageSpan))))
                    {
                        if (spannable.GetSpanStart(span) >= index && spannable.GetSpanEnd(span) <= index + smiley.Length)
                            spannable.RemoveSpan(span);
                        else
                        {
                            set = false;
                            break;
                        }
                    }
                    if (set)
                    {
                        hasChanges = true;
                        spannable.SetSpan(new ImageSpan(context, smileyImage), index, index + smiley.Length, SpanTypes.ExclusiveExclusive);
                    }
                }
            }
            return hasChanges;
        }

        public ISpannable GetSmiledText(Context context, ICharSequence text)
        {
            var spannable = SpannableFactory.Instance.NewSpannable(text);
            AddSmiles(context, spannable);
            return spannable;
        }


        public void AddSmiley(string textSmiley, int smileyResource)
        {
            Emoticons.Add(textSmiley, smileyResource);
        }
    }
    public static class StringExtensions
    {
        public static IEnumerable<int> IndexesOf(this string haystack, string needle)
        {
            var lastIndex = 0;
            while (true)
            {
                var index = haystack.IndexOf(needle, lastIndex);
                if (index == -1)
                {
                    yield break;
                }
                yield return index;
                lastIndex = index + needle.Length;
            }
        }
    }
}