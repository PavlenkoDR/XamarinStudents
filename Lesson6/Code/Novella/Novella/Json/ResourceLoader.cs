using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Novella.Json
{
    public class ResourceLoader
    {
        SortedDictionary<string, object> resources = new SortedDictionary<string, object>();
        List<Task> tasks = new List<Task>();
        public ResourceLoader()
        {
            if (File.Exists("profile.json"))
            {
                try
                {
                    var json = File.ReadAllText("profile.json");
                    Profile.Instance = JsonConvert.DeserializeObject<Profile>(json);
                }
                catch
                {
                }
            }

            var allResources = GetType().Assembly.GetManifestResourceNames();
            var dialogLoader = new Loader<Dialog>();
            var imageSourceLoader = new Loader<ImageSource>();
            var profileLoader = new Loader<Profile>();
            var backroundLoader = new Loader<Background>();
            foreach (var resource in allResources)
            {
                tasks.Add(Task.Run(() =>
                {
                    if (resource.Contains("Assets.Dialogs"))
                    {
                        resources.Add(resource, dialogLoader.LoadJson(resource));
                    }
                    else if (resource.Contains("Assets.Images"))
                    {
                        resources.Add(resource, imageSourceLoader.LoadImageSource(resource));
                    }
                    else if (resource.Contains("Assets.profile.json"))
                    {
                        var defaultProfile = profileLoader.LoadJson(resource);
                        Profile.Instance = Profile.Instance ?? defaultProfile;
                        resources.Add(resource, defaultProfile);
                    }
            }));
        }
        }

        public class Loader<T> where T : class
        {
            public T LoadJson(string path)
            {
                try
                {
                    var stream = GetType().Assembly.GetManifestResourceStream(path);
                    var reader = new StreamReader(stream);
                    var json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch { }
                return null;
            }

            public T LoadImageSource(string path)
            {
                return (ImageSource.FromResource(path)) as T;
            }
        }

        public bool isLoading()
        {
            foreach (var task in tasks)
            {
                if (!task.IsCompleted)
                {
                    return true;
                }
            }
            return false;
        }

        public T Load<T>(string name) where T : class
        {
            if (name != null && resources.TryGetValue(name, out var value))
            {
                return value as T;
            }
            return default(T);
        }

        public void Wait()
        {
            Task.WaitAll(tasks.ToArray());
        }

        public Task WhenAll()
        {
            return Task.WhenAll(tasks.ToArray());
        }
    }

    public static class ResourceLoaderInstance
    {
        public static ResourceLoader instance = null;
        public static ResourceLoader Instance
        {
            get
            {
                instance = instance ?? new ResourceLoader();
                return instance;
            }
        }
    }
}
