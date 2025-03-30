namespace Dig.Models;

public class Notification(long id, string type, string content, DateTime dateTime)
{
    public long Id { get; set; } = id;
    public string Type { get; set; } = type;
    public string Content { get; set; } = content;
    public DateTime DateTime { get; set; } = dateTime;
}