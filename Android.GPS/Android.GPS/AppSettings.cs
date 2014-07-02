using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OsmSharp.Math.Geo;

namespace Android.GPS
{
    public static class AppSettings
    {
        public static GeoCoordinate Location { get; set; }

        public static float Zoom { get; set; }
    }
}