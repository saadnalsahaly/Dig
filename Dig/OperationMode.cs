namespace Dig;

public class OperationMode(long id, string mode, DateTime dateTime)
{
    public long Id { get; set; } = id;
    public string Mode { get; set; } = mode;
    public DateTime DateTime { get; set; } = dateTime;
}