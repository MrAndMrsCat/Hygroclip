using System.ComponentModel.DataAnnotations;

namespace HomeAutomationServer.Data;

public class EnvironmentalMeasurement
{
    [Key]
    public long ID { get; set; }
    public DateTime DateTime { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
}
