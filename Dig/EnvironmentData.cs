namespace Dig;

public class EnvironmentData(long id, double temperature, double humidity, double lightIntensity, DateTime dateTime)
{
    public long Id { get; set; } = id;
    public double Temperature { get; set; } = temperature;
    public double Humidity { get; set; } = humidity;
    public double LightIntensity { get; set; } = lightIntensity;
    public DateTime DateTime { get; set; } = dateTime;
}