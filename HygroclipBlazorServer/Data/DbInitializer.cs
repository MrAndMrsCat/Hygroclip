using HygroclipBlazorServer.Devices;

namespace HygroclipBlazorServer.Data
{
    public static class DbInitializer
    {
        public static void Initialize(EnvironmentalMeasurementContext context)
        {
            context.Database.EnsureCreated();


//            if (context.MeasurementData is not null)
//            {
//                if (context.MeasurementData.Any()) return;

//#if DEBUG
//                context.MeasurementData.AddRange(
//                    new EnvironmentalMeasurement()
//                    {
//                        ID = 1,
//                        DateTime = new DateTime(2022, 9, 19, 15, 30, 00),
//                        Temperature = 24,
//                        Humidity = 44,
//                    },
//                    new EnvironmentalMeasurement()
//                    {
//                        ID = 2,
//                        DateTime = new DateTime(2022, 9, 19, 15, 40, 00),
//                        Temperature = 26,
//                        Humidity = 42,
//                    },
//                    new EnvironmentalMeasurement()
//                    {
//                        ID = 3,
//                        DateTime = new DateTime(2022, 9, 19, 15, 50, 00),
//                        Temperature = 20,
//                        Humidity = 50,
//                    }

//                    );

//                context.SaveChangesAsync();
//#endif
//            }
        }
    }
}
