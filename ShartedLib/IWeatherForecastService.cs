namespace ShartedLib {
    public interface IWeatherForecastService {
        IEnumerable<WeatherForecast> GetForcasts(int linit = 5);
    }
}