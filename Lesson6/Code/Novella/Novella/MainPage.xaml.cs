using Newtonsoft.Json;
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
            Load();
        }

        private void LoadBackGrounds(ref List<Background> backgrounds)
        {
            if (File.Exists("background.json"))
            {
                try
                {
                    var json = File.ReadAllText("background.json");

                    backgrounds = JsonConvert.DeserializeObject<List<Background>>(json);
                }
                catch
                {
                }
            }
        }
        private string getBackgroundPath(string name, List<Background> backgrounds)
        {
            foreach (var background in backgrounds)
            {
                if (background.name == name)
                {
                    return background.image;
                }
            }

            return null;
        }

        private async void Load()
        {
            await ResourceLoaderInstance.Instance.WhenAll();
            dialog = ResourceLoaderInstance.Instance.Load<Dialog>($"Novella.Assets.Dialogs.{Profile.Instance.active}.json");


            List<Background> backgrounds = new List<Background>();
            // var jbackgrounds = ResourceLoaderInstance.Instance. // TODO that

            backgroundImage.Source = ResourceLoaderInstance.Instance.Load<ImageSource>(getBackgroundPath(dialog.background, backgrounds));


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
