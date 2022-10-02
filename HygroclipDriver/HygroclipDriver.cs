using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HygroclipDriver
{
    public class HygroclipDriver
    {
        public class HygroClipMeasurment : EventArgs
        {
            public double Humidity { get; init; }
            public double Temperature { get; init; }
        }

        public HygroclipDriver(ISerialComms communicationInterface)
        {
            _commsInterface = communicationInterface;

            _commsInterface.ReceivedBytes += (s, bytes) =>
            {
                try
                {
                    string[] values = Encoding.ASCII.GetString(bytes).Split(';');

                    foreach (Status index in Enum.GetValues<Status>())
                    {
                        string value = values[(int)index];

                        switch (index)
                        {
                            case Status.RelativeHumidity:
                                Humidity = double.Parse(value);
                                break;

                            case Status.Temperature:
                                Temperature = double.Parse(value);
                                break;

                            case Status.FirmwareVersion:
                                FirmwareVersion = value;
                                break;

                            case Status.SerialNumber:
                                SerialNumber = value;
                                break;
                        }
                    }

                    NewMeasurement?.Invoke(this, new HygroClipMeasurment()
                    {
                        Humidity = Humidity,
                        Temperature = Temperature
                    });
                }
                catch (Exception ex)
                {
                    Serilog.Log.ForContext<HygroclipDriver>().Error(ex.Message);
                }
            };

            _pollingTimer.Elapsed += (s, e) => GetStatus();
        }

        private readonly ISerialComms _commsInterface;
        private readonly System.Timers.Timer _pollingTimer = new() { Interval = 200 };

        public event EventHandler<HygroClipMeasurment>? NewMeasurement;
        enum Status
        {
            RelativeHumidity = 1,
            Temperature = 5,
            FirmwareVersion = 15,
            SerialNumber = 16
        }

        public double Humidity { get; private set; }
        public double Temperature { get; private set; }
        public string FirmwareVersion { get; private set; } = "";
        public string SerialNumber { get; private set; } = "";
        public double PollingInterval
        {
            get => _pollingTimer.Interval;
            set => _pollingTimer.Interval = value;
        }
        private byte[] GetStatusCommand { get; } = Encoding.ASCII.GetBytes("{ 99RDD}\r");

        private void GetStatus() => _commsInterface.Send(GetStatusCommand);
        public void StartPolling() => _pollingTimer.Start();
        public void StopPolling() => _pollingTimer.Stop();
        public void Halt() => _commsInterface.Halt();
        
    }
}
