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
using Newtonsoft.Json;

namespace UltimateKisan
{
    public class Weather
    {
        public string Title { get; set; }
        public string Temperature { get; set; }
        public string Wind { get; set; }
        public string Humidity { get; set; }
        public string Visibility { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Description { get; set; }

        public Weather()
        {
            this.Title = " ";
            this.Temperature = "NA";
            this.Wind = "NA";
            this.Humidity = "NA";
            this.Visibility = "NA";
            this.Sunrise = "NA";
            this.Sunset = "NA";
            this.Description = "NA";
        }
    }
}  