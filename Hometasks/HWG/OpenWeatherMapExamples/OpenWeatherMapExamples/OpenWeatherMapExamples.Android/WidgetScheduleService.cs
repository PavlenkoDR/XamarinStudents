using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Widget;
using System.Linq;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;

namespace OpenWeatherMapExamples.Droid
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    public class WidgetScheduleService:RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            return new WidgetScheduleFactory(this.ApplicationContext);
        }
    }

    public class WidgetScheduleFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        public int Count => _source.Count;

        public bool HasStableIds => true;

        public RemoteViews LoadingView => null;

        public int ViewTypeCount => 1;

        List<WeatherByTime> _source;
        Context _context;
        IWebApi _webApi;

        public WidgetScheduleFactory(Context context)
        {
            _context = context;
            _webApi = new WebApi();
        }

        public long GetItemId(int position)
        {
            return position;
        }

        public RemoteViews GetViewAt(int position)
        {
            if (_source.Count == 0)
            {
                return null;
            }

            var forecastData = _source[position];

            var remoteViews = new RemoteViews(_context.PackageName, Resource.Layout.WidgetCell);

            remoteViews.SetTextViewText(Resource.Id.widgetcell_description, forecastData.description);
            remoteViews.SetTextViewText(Resource.Id.widgetcell_time, forecastData.time);

            var handler = new ImageLoaderSourceHandler();
            var data = handler.LoadImageAsync(forecastData.icon, Application.Context);
            Task.WaitAll(data);
            remoteViews.SetImageViewBitmap(Resource.Id.widgetcell_image, data.Result);

            var intent = new Intent();
            remoteViews.SetOnClickFillInIntent(Resource.Id.widgetcell_container, intent);

            return remoteViews;
        }

        public void OnCreate()
        {
            System.Diagnostics.Debug.WriteLine("OnCreate:");
        }

        public void OnDataSetChanged()
        {
            System.Diagnostics.Debug.WriteLine("OnDataSetChanged:");
            _source = GetData();
            System.Diagnostics.Debug.WriteLine(_source);
        }

        List<WeatherByTime> GetData()
        {
            var task = _webApi.GetForecast();
            Task.WaitAll(task);
            return task.Result;
        }


        public void OnDestroy()
        {

        }
    }
}
