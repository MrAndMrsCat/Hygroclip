using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HygroclipDriver
{
    public class HygroclipDevice
    {
        public HygroclipDevice(HygroclipDriver driver)
        {
            Driver = driver;

            InitializeDriver(Driver);
        }

        public HygroclipDevice(string comportName)
        {
            SerialPortDevice port = new(new()
            {
                PortName = comportName,
                BaudRate = 19200,
                TerminationCharacter = '\r'
            });

            Driver = new HygroclipDriver(port);

            InitializeDriver(Driver);
        }

        /// <summary> Simulated </summary>
        public HygroclipDevice()
        {
            Driver = new(new SimulatedHygroclipSerialPort());

            InitializeDriver(Driver);
        }

        public event EventHandler<HygroclipDriver.HygroClipMeasurment>? NewMeasurement;

        public double PollingInterval
        {
            get => Driver.PollingInterval;
            set => Driver.PollingInterval = value;
        }
        private HygroclipDriver Driver { get; }

        public double Humidity => _humidityFilter.Value;
        public double Temperature => _temperatureFilter.Value;
        private readonly IIRFilter _humidityFilter = new(0.25);
        private readonly IIRFilter _temperatureFilter = new(0.25);

        private void InitializeDriver(HygroclipDriver driver)
        {
            driver.NewMeasurement += (s, measurement) =>
            {
                _humidityFilter.Input(measurement.Humidity);
                _temperatureFilter.Input(measurement.Temperature);

                Serilog.Log.ForContext<HygroclipDevice>().Debug(Status);

                NewMeasurement?.Invoke(this, new HygroclipDriver.HygroClipMeasurment()
                {
                    Humidity = Humidity,
                    Temperature = Temperature
                });
            };

            driver.StartPolling();
        }

        public void Halt() => Driver.Halt();


        public string Status => $"Temp: {Temperature:0.0}C, RH: {Humidity:0.00}%";
        public override string ToString() => $"Hygroclip\n{Status}";
    }
}
