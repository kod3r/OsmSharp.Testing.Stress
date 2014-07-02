using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using OsmSharp.Android.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.Math.Geo;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using OsmSharp.Android.UI.Data.SQLite;
using System.Reflection;

namespace Android.GPS
{
    [Activity(Label = "Android.GPS", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity, ILocationListener
    {
        /// <summary>
        /// Holds the mapview.
        /// </summary>
        private MapView _mapView;

        /// <summary>
        /// Holds the marker.
        /// </summary>
        private MapMarker _mapMarker;

        /// <summary>
        /// Holds the location manager.
        /// </summary>
        private LocationManager _locationManager; 
        
        /// <summary>
        /// Holds the location provider.
        /// </summary>
        private string _locationProvider;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // hide title bar.
            this.RequestWindowFeature(global::Android.Views.WindowFeatures.NoTitle);

            // initialize OsmSharp native handlers.
            Native.Initialize();

            // initialize map.
            var map = new Map();

            // add tile Layer.
            // WARNING: Always look at usage policies!
            // WARNING: Don't use my tiles, it's a free account and will shutdown when overused!
            map.AddLayer(new LayerTile("http://a.tiles.mapbox.com/v3/osmsharp.i8ckml0l/{0}/{1}/{2}.png"));

            // define the mapview.
            _mapView = new MapView(this, new MapViewSurface(this));
            _mapView.Map = map;
            _mapView.MapMaxZoomLevel = 18; // limit min/max zoom because MBTiles sample only contains a small portion of a map.
            _mapView.MapMinZoomLevel = 0;
            _mapView.MapTilt = 0;
            _mapView.MapCenter = new GeoCoordinate(51.0992, 4.5374);
            _mapView.MapZoom = 17;
            _mapView.MapAllowTilt = false;
            _mapView.MapScaleFactor = 3;

            // create marker.
            _mapMarker = new MapMarker(this, _mapView.MapCenter);
            _mapView.AddMarker(_mapMarker);

            AppSettings.Location = _mapMarker.Location;
            AppSettings.Zoom = _mapView.MapZoom;

            // initialize the location manager.
            InitializeLocationManager();

            // set the map view as the default content view.
            SetContentView(_mapView);
        }

        protected override void OnPause()
        {
            // remove updates.
            _locationManager.RemoveUpdates(this);

            // pause the mapview (be sure to do this after remove updates).
            _mapView.Pause();

            AppSettings.Location = _mapMarker.Location;
            AppSettings.Zoom = _mapView.MapZoom;

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            // resume the mapview.
            _mapView.Resume();

            if(_mapMarker != null &&
                AppSettings.Location != null)
            _mapMarker.Location = AppSettings.Location;
            _mapView.MapZoom = AppSettings.Zoom;

            // add update locations again.
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // dispose of all resources.
            // the mapview is completely destroyed in this sample, read about the Android Activity Lifecycle here:
            // http://docs.xamarin.com/guides/android/application_fundamentals/activity_lifecycle/
            _mapView.Dispose();
            GC.Collect();
        }

        #region Location Listener Implementation

        /// <summary>
        /// Initializes the location manager.
        /// </summary>
        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = String.Empty;
            }
        }

        /// <summary>
        /// Called when the location has changed.
        /// </summary>
        /// <param name="location"></param>
        public void OnLocationChanged(Location location)
        {
            _mapMarker.Location = new GeoCoordinate(location.Latitude, location.Longitude);

            _mapView.MapCenter = _mapMarker.Location;
        }

        /// <summary>
        /// Called when the provider is disabled by the user.
        /// </summary>
        /// <param name="provider"></param>
        public void OnProviderDisabled(string provider)
        {
            _mapView.RemoveMarker(_mapMarker);
        }

        /// <summary>
        /// Called when the provider is enabled by the user.
        /// </summary>
        /// <param name="provider"></param>
        public void OnProviderEnabled(string provider)
        {
            _mapView.AddMarker(_mapMarker);
        }

        /// <summary>
        /// Called when the provider status changes.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="status"></param>
        /// <param name="extras"></param>
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            // do nothing! yay!
        }

        #endregion
    }
}