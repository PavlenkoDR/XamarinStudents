using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OpenWeatherMapExamples
{
    public class Coordinates
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }
    public class CityDescription
    {
        public int id { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public Coordinates coord { get; set; }
    }

    public static class CityLoader
    {
        public static List<CityDescription> cityList { get; private set; } = null;
        public static Task<List<CityDescription>> cityListTask { get; private set; } = null;

        private static bool cityLoadingIsAsync = true;

        private static List<CityDescription> LoadJsonDeserializeObject()
        {
            var stream = Application.Current.GetType().Assembly.GetManifestResourceStream("OpenWeatherMapExamples.city.list.json");
            var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<CityDescription>>(json);
        }
        private static Task<List<CityDescription>> LoadJsonDeserializeObjectAsync()
        {
            return Task.Run(() =>
            {
                return LoadJsonDeserializeObject();
            });

        }

        public static void InitializeCity()
        {
            var startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (cityLoadingIsAsync)
            {
                cityListTask = LoadJsonDeserializeObjectAsync();
                cityListTask.ContinueWith((Task<List<CityDescription>> currentTask) =>
                {
                    cityList = currentTask.Result;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Msg", $"Full loaded in {DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime} milliseconds", "OK");
                    });
                });
            }
            else
            {
                cityListTask = Task.Run(() => { return new List<CityDescription>(); });
                cityList = LoadJsonDeserializeObject();
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Msg", $"Loaded in {DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime} milliseconds", "OK");
            });
        }
    }
}
