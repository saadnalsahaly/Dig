namespace Dig;

public class UserCommand(long id, double temperature, double humidity, double lightIntensity)
{
    public long Id { get; set; } = id;
    public double Temperature { get; set; } = temperature;
    public double Humidity { get; set; } = humidity;
    public double LightIntensity { get; set; } = lightIntensity;
}