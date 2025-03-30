namespace Dig.Models;

public class Plant(long id, string plantId, DateTime dateTime)
{
    public long Id { get; set; } = id;
    public string PlantId { get; set; } = plantId;
    public DateTime DateTime { get; set; } = dateTime;
}