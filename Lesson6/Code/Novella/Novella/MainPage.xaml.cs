using Novella.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Novella
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            var imageSource = ImageSource.FromResource(Source);

            return imageSource;
        }
    }

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        
        Dialog dialog = null;
        int option = 0;
        public MainPage()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                Load();
            });
        }

        private async void Load()
        {
            await ResourceLoaderInstance.Instance.WhenAll();
            dialog = ResourceLoaderInstance.Instance.Load<Dialog>($"Novella.Assets.Dialogs.{Profile.Instance.active}.json");
            backgroundImage.Source = ResourceLoaderInstance.Instance.Load<ImageSource>($"Novella.Assets.Images.Background.{dialog.background}");
            npcLeftActive.Source = ResourceLoaderInstance.Instance.Load<ImageSource>("Novella.Assets.Images.NPC." + dialog.steps[option].npc + ".png");
            mainText.Text = dialog.steps.First()?.text ?? "";
            ++option;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (option == dialog.steps.Count)
            {
                return;
            }
            mainText.Text = dialog.steps[option].text;
            npcLeftActive.Source = ResourceLoaderInstance.Instance.Load<ImageSource>("Novella.Assets.Images.NPC." + dialog.steps[option].npc + ".png");
            ++option;
        }
    }
}
