namespace Dig.Models;

public class Notification
{
    public long Id { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public DateTime DateTime { get; set; }
}