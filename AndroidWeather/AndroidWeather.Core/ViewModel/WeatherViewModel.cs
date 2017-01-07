using Weather.Helpers;
using Weather.Models;
using Weather.Services;
using Plugin.Geolocator;
//using Plugin.TextToSpeech;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Weather.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        WeatherService WeatherService { get; } = new WeatherService();

        string location = Settings.City;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                OnPropertyChanged();
                Settings.City = value;
            }
        }

        bool useGPS;
        public bool UseGPS
        {
            get { return useGPS; }
            set
            {
                useGPS = value;
                OnPropertyChanged();
            }
        }

        bool isImperial = Settings.IsImperial;
        public bool IsImperial
        {
            get { return isImperial; }
            set
            {
                isImperial = value;
                OnPropertyChanged();
                Settings.IsImperial = value;
            }
        }

        string temp = string.Empty;
        public string Temp
        {
            get { return temp; }
            set { temp = value; OnPropertyChanged(); }
        }

        string condition = string.Empty;
        public string Condition
        {
            get { return condition; }
            set { condition = value; OnPropertyChanged(); }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        public bool IsDataAvailable
        {
            get { return Forecast != null; }
        }

        //public ImageSource BackgroundImageSource
        //{
        //    get
        //    {
        //        if(Condition.ToLower().Contains("sun") || Condition.ToLower().Contains("clear"))
        //            return ImageSource.FromResource("MyWeather.Images.sun.jpg");
        //        if (Condition.ToLower().Contains("snow"))
        //            return ImageSource.FromResource("MyWeather.Images.snow.jpg");
        //        if (Condition.ToLower().Contains("rain"))
        //            return ImageSource.FromResource("MyWeather.Images.rain.jpg");
        //        return ImageSource.FromResource("MyWeather.Images.cloud.jpg");
        //    }
        //}

        WeatherForecastRoot forecast;
        public WeatherForecastRoot Forecast
        {
            get { return forecast; }
            set { forecast = value; OnPropertyChanged(); OnPropertyChanged("IsDataAvailable"); OnPropertyChanged("BackgroundImageSource"); }
        }

        //ICommand getWeather;
        //public ICommand GetWeatherCommand =>
        //        getWeather ??
        //        (getWeather = new Command(async () => await ExecuteGetWeatherCommand()));

        public async Task ExecuteGetWeatherCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                WeatherRoot weatherRoot = null;
                var units = IsImperial ? Units.Imperial : Units.Metric;

                if (UseGPS)
                {
                    var gps = await CrossGeolocator.Current.GetPositionAsync(10000);
                    weatherRoot = await WeatherService.GetWeather(gps.Latitude, gps.Longitude, units);
                }
                else
                {
                    //Get weather by city
                    weatherRoot = await WeatherService.GetWeather(Location.Trim(), units);
                }
                
                //Get forecast based on cityId
                Forecast = await WeatherService.GetForecast(weatherRoot.CityId, units);

                var unit = IsImperial ? "F" : "C";
                var temp = (int)(weatherRoot?.MainWeather?.Temperature ?? 0);
  
                Temp = $"{temp}°{unit}";
                Condition = $"{weatherRoot?.Weather?[0]?.Description ?? string.Empty}";

                //CrossTextToSpeech.Current.Speak(Temp + " " + Condition);
            }
            catch
            {
                Temp = "Unable to get Weather";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
