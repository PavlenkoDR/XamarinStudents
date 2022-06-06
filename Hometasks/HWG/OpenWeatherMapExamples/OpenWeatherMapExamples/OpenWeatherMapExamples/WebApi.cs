using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace OpenWeatherMapExamples
{
    public interface IWebApi
    {
        Task<List<WeatherByTime>> GetForecast();
    }

    public struct WeatherByTime
    {
        public string time { get; set; }
        public string description { get; set; }
        public UriImageSource icon { get; set; }
    }

    public class WebApi : IWebApi
    {
        public static readonly string exampleForecastUrl = "https://samples.openweathermap.org/data/2.5/forecast?id=524901&appid=b1b15e88fa797225412429c1c50c122a1";

        internal static HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(180)
        };

        SortedDictionary<string, UriImageSource> loadCache = new SortedDictionary<string, UriImageSource>();

        public WebApi()
        {
        }

        public async Task<List<WeatherByTime>> GetForecast()
        {
            var forecast = await RequestObserver(exampleForecastUrl);
            var dataList = new List<WeatherByTime>();
            foreach (var data in forecast.list)
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                dateTime = dateTime.AddSeconds((double)data.dt);
                var url = $"https://openweathermap.org/img/wn/{data.weather[0].icon}@2x.png";
                if (!loadCache.ContainsKey(url))
                {
                    loadCache.Add(url, new UriImageSource { Uri = new Uri(url) });
                }
                dataList.Add(new WeatherByTime()
                {
                    time = $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}",
                    description = data.weather[0].description,
                    icon = loadCache[url]
                });
            }
            return dataList;
        }

        internal async Task<dynamic> RequestObserver(string url, int retryCount = 2)
        {
            while (retryCount + 1 > 0)
            {
                try
                {
                    var response = await httpClient.GetAsync(url).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        dynamic obj = JsonConvert.DeserializeObject(content);
                        return obj;
                    }
                    --retryCount;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return null;
        }
    }
}
