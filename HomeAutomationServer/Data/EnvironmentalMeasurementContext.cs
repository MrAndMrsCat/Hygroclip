using Microsoft.EntityFrameworkCore;

namespace HomeAutomationServer.Data
{
    public class EnvironmentalMeasurementContext : DbContext
    {
        public EnvironmentalMeasurementContext(DbContextOptions<EnvironmentalMeasurementContext> options) : base(options)
        {
        }

        public DbSet<EnvironmentalMeasurement>? MeasurementData { get; set; }
    }
}
