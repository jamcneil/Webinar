using Weather.Models;
using System.Net.Http;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace Weather.Services
{
    public enum Units
    {
        Imperial,
        Metric
    }

    public class WeatherService
    {
        const string WeatherCoordinatesUri = "http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&units={2}&appid=[Your ID Here]“;
        const string WeatherCityUri = "http://api.openweathermap.org/data/2.5/weather?q={0}&units={1}&appid=[Your ID Here]“;
        const string ForecaseUri = "http://api.openweathermap.org/data/2.5/forecast?id={0}&units={1}&appid=[Your ID Here]“;

        public async Task<WeatherRoot> GetWeather(double latitude, double longitude, Units units = Units.Imperial)
        {
            using (var client = new HttpClient())
            {
                var url = string.Format(WeatherCoordinatesUri, latitude, longitude, units.ToString().ToLower());
                var json = await client.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return DeserializeObject<WeatherRoot>(json);
            }
        }

        public async Task<WeatherRoot> GetWeather(string city, Units units = Units.Imperial)
        {
            using (var client = new HttpClient())
            {
                var url = string.Format(WeatherCityUri, city, units.ToString().ToLower());
                var json = await client.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return DeserializeObject<WeatherRoot>(json);
            }
        }

        public async Task<WeatherForecastRoot> GetForecast(int id, Units units = Units.Imperial)
        {
            using (var client = new HttpClient())
            {
                var url = string.Format(ForecaseUri, id, units.ToString().ToLower());
                var json = await client.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return DeserializeObject<WeatherForecastRoot>(json);
            }
        }
    }
}