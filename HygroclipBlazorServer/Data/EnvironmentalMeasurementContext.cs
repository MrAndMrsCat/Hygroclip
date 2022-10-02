using Microsoft.EntityFrameworkCore;

namespace HygroclipBlazorServer.Data
{
    public class EnvironmentalMeasurementContext : DbContext
    {
        public EnvironmentalMeasurementContext(DbContextOptions<EnvironmentalMeasurementContext> options) : base(options)
        {
        }

        public DbSet<EnvironmentalMeasurement>? MeasurementData { get; set; }
    }
}
