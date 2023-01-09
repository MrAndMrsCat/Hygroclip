using static PicoController.PicoBoilerTimerControlPolicy;

namespace HomeAutomationServer.Data
{
    public class AppConfiguration
    {
        const string DefaultFilepath = "/etc/homeautomation/app_config.xml";
        public static AppConfiguration Instance { get; set; } = Default();
        public float TemperatureSetpoint { get; set; } 
        public ControlMode ControlMode { get; set; } 
        public List<Setpoint> TimerSetpoints { get; init; } = new();
        public static AppConfiguration Default()
        {
            return new AppConfiguration()
            {
                TemperatureSetpoint = 13,
                ControlMode= ControlMode.Timed,
                TimerSetpoints = new()
                {
                    new Setpoint(DayOfWeek.Monday, 7, 30, true),
                    new Setpoint(DayOfWeek.Monday, 21, 0, false),
                    new Setpoint(DayOfWeek.Tuesday, 7, 30, true),
                    new Setpoint(DayOfWeek.Tuesday, 8, 15, false),
                    new Setpoint(DayOfWeek.Tuesday, 15, 30, true),
                    new Setpoint(DayOfWeek.Tuesday, 21, 30, false),
                    new Setpoint(DayOfWeek.Wednesday, 7, 30, true),
                    new Setpoint(DayOfWeek.Wednesday, 21, 30, false),
                    new Setpoint(DayOfWeek.Thursday, 7, 30, true),
                    new Setpoint(DayOfWeek.Thursday, 8, 15, false),
                    new Setpoint(DayOfWeek.Thursday, 15, 30, true),
                    new Setpoint(DayOfWeek.Thursday, 21,30, false),
                    new Setpoint(DayOfWeek.Friday, 7, 30, true),
                    new Setpoint(DayOfWeek.Friday, 21, 30, false),
                    new Setpoint(DayOfWeek.Saturday, 7, 30, true),
                    new Setpoint(DayOfWeek.Saturday, 21, 30, false),
                    new Setpoint(DayOfWeek.Sunday, 7, 30, true),
                    new Setpoint(DayOfWeek.Sunday, 21, 30, false),
                }
            };
        }

        internal static void Load(string filepath = DefaultFilepath)
        {
            if (XMLConfigurationSerializer<AppConfiguration>.TryDeserialize(filepath, out AppConfiguration config))
            {
                Instance = config;
            }
        }

        internal static void Save(string filepath = DefaultFilepath)
        {
            try
            {
                Directory.CreateDirectory($"{Path.GetDirectoryName(filepath)}");
                XMLConfigurationSerializer<AppConfiguration>.Serialize(Instance, filepath);
            }
            catch (Exception ex)
            {
                Serilog.Log.Logger.Error(ex.Message);
            }
            
        }
    }
}
