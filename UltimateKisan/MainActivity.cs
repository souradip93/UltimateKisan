using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using System;
using Xamarin.Essentials;
using System.Resources;
using Android.Views;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using System.Text;

namespace UltimateKisan
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        GoogleMap mmap;

        const string COGNITIVE_SERVICES_KEY = "<KEY>";
        const string COGNITIVE_SPEECH_SERVICES_KEY = "<KEY>";
        const string OPEN_WEATHER_API_KEY = "<KEY>";
        const string REGION = "westeurope";
        // Endpoints for Translator Text and Bing Spell Check
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";

        string currentLanguage = "en";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, 0);

        }

        private async void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("Selected language is {0}", spinner.GetItemAtPosition(e.Position));

            string toLanguage = string.Format("{0}", spinner.GetItemAtPosition(e.Position));

            if (toLanguage == "English")
                toLanguage = "en";
            else if (toLanguage == "Hindi")
                toLanguage = "hi";
            else if (toLanguage == "Marathi")
                toLanguage = "mr";

            string[] results = await GetTranslatedText(toLanguage);

            var temperatureSummary = FindViewById<TextView>(Resource.Id.temperatureSummary);
            var soilSummary = FindViewById<TextView>(Resource.Id.soilSummary);
            var areaSummary = FindViewById<TextView>(Resource.Id.areaSummary);

            temperatureSummary.Text = results[0];
            soilSummary.Text = results[1];
            areaSummary.Text = results[2];

            currentLanguage = toLanguage;
        }

        public async Task<string[]> GetTranslatedText(string toLanguage)
        {
            // Send translation request
            string endpoint = string.Format(TEXT_TRANSLATION_API_ENDPOINT, "translate");
            string uri = string.Format(endpoint + "&from={0}&to={1}", currentLanguage, toLanguage);

            var temperatureSummary = FindViewById<TextView>(Resource.Id.temperatureSummary);
            var soilSummary = FindViewById<TextView>(Resource.Id.soilSummary);
            var areaSummary = FindViewById<TextView>(Resource.Id.areaSummary);

            System.Object[] body = new System.Object[] { new { Text = temperatureSummary.Text }, new { Text = soilSummary.Text }, new { Text = areaSummary.Text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
                request.Headers.Add("Ocp-Apim-Subscription-Region", REGION);
                request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var responseResult = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);
                string[] translation = new string[] { responseResult[0]["translations"][0]["text"], responseResult[1]["translations"][0]["text"], responseResult[2]["translations"][0]["text"] };
                return translation;
            }
        }

        public async void SpeechButtonClicked(object sender, EventArgs args)
        {

            var translation = await GetTranslatedText("hi");

            var config = SpeechConfig.FromSubscription(COGNITIVE_SPEECH_SERVICES_KEY, REGION);
            config.SpeechSynthesisLanguage = "hi-IN";
            using (var synthesizer = new SpeechSynthesizer(config))
            {

                using (var result = await synthesizer.SpeakTextAsync(translation[0]))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        //success;
                    }
                }

                using (var result = await synthesizer.SpeakTextAsync(translation[1]))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        //success;
                    }
                }

                using (var result = await synthesizer.SpeakTextAsync(translation[2]))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        //success;
                    }
                }

            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // set the menu layout on Main Activity  
            MenuInflater.Inflate(Resource.Menu.mainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuItem1:
                    {
                        // add your code  
                        return true;
                    }
                case Resource.Id.menuItem2:
                    {
                        // add your code
                        return true;
                    }
                case Resource.Id.menuItem3:
                    {
                        // add your code  
                        return true;
                    }
            }
            return base.OnOptionsItemSelected(item);
        }

        public async void OnMapReady(GoogleMap map)
        {
            mmap = map;

            mmap.MyLocationEnabled = true;
            mmap.MapType = GoogleMap.MapTypeHybrid;
            mmap.UiSettings.ZoomControlsEnabled = true;

            //Set the farm positions
            PolygonOptions rectOptions = new PolygonOptions();
            rectOptions.Add(new LatLng(11.838600285438176, 75.62577920991211));
            rectOptions.Add(new LatLng(11.83865591985375, 75.62651107020247));
            rectOptions.Add(new LatLng(11.838092620878797, 75.6262055362958));
            rectOptions.Add(new LatLng(11.838092620881227, 75.62620529054104));
            rectOptions.Add(new LatLng(11.838252570075255, 75.6257434369438));
            rectOptions.Add(new LatLng(11.838600285438176, 75.62577920991211));

            // Market settings
            rectOptions.InvokeFillColor(int.Parse("6ca5ce59", System.Globalization.NumberStyles.HexNumber));
            rectOptions.InvokeStrokeWidth(1);

            // Add polygon to map
            mmap.AddPolygon(rectOptions);

            // Set the current camera position
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Zoom(19);
            LatLng tempLocation = new LatLng(11.83865591985375, 75.6262055362958);
            builder.Target(tempLocation);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            mmap.MoveCamera(cameraUpdate);

            // Get weather details
            Weather weather = await GetWeather(11.83865591985375, 75.6262055362958);
            TextView weatherTV = FindViewById<TextView>(Resource.Id.weather);
            TextView temperature = FindViewById<TextView>(Resource.Id.temperature);
            TextView wind = FindViewById<TextView>(Resource.Id.wind);
            TextView humidity = FindViewById<TextView>(Resource.Id.humidity);
            TextView visibility = FindViewById<TextView>(Resource.Id.visibility);

            // Set weather details
            weatherTV.Text = weather.Description;
            temperature.Text = weather.Temperature;
            wind.Text = weather.Wind;
            humidity.Text = weather.Humidity;
            visibility.Text = weather.Visibility;

            // Set Status image
            var temperatureSummaryStatus = FindViewById<ImageView>(Resource.Id.temperatureSummaryStatus);
            temperatureSummaryStatus.SetImageResource(Resource.Drawable.toggle_checked);

            var soilSummaryStatus = FindViewById<ImageView>(Resource.Id.soilSummaryStatus);
            soilSummaryStatus.SetImageResource(Resource.Drawable.toggle_checked);

            var areaSummaryStatus = FindViewById<ImageView>(Resource.Id.areaSummaryStatus);
            areaSummaryStatus.SetImageResource(Resource.Drawable.toggle_checked);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            Button speechBtn = FindViewById<Button>(Resource.Id.speechBtn);
            speechBtn.Click += SpeechButtonClicked;

            Spinner spinner = FindViewById<Spinner>(Resource.Id.lngSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter<string>.CreateFromResource(this, Resource.Array.ln_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public static async Task<Weather> GetWeather(double lat, double lon)
        {
            string key = OPEN_WEATHER_API_KEY;
            string queryString = "https://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + "&appid=" + key;
            dynamic results = await getDataFromService(queryString).ConfigureAwait(false);
            if (results["weather"] != null)
            {
                Weather weather = new Weather();
                weather.Title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase((string)results["name"]);
                weather.Temperature = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase((string)results["main"]["temp"] + " F");
                weather.Description = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase((string)results["weather"][0]["description"]);
                DateTime time = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime sunrise = time.AddSeconds((double)results["sys"]["sunrise"]);
                DateTime sunset = time.AddSeconds((double)results["sys"]["sunset"]);
                weather.Sunrise = sunrise.ToString() + " UTC";
                weather.Sunset = sunset.ToString() + " UTC";
                weather.Wind = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase((string)results["wind"]["speed"] + " mph");
                weather.Humidity = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase((string)results["main"]["humidity"] + " %");
                return weather;
            }
            else
            {
                return null;
            }
        }
        public static async Task<dynamic> getDataFromService(string pQueryString)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(pQueryString);
            dynamic data = null;
            if (response != null)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                data = JsonConvert.DeserializeObject(json);
            }
            return data;
        }

    }
}