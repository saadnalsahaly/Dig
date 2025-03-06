namespace Dig;

public class PlantStatus(long id, string plantId, string disease, double confidence, DateTime dateTime)
{
    public long Id { get; set; } = id;
    public string PlantId { get; set; } = plantId;
    public string Disease { get; set; } = disease;
    public double Confidence { get; set; } = confidence;
    public DateTime DateTime { get; set; } = dateTime;
}