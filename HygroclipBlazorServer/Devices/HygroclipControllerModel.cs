using HygroclipDriver;
using HygroclipBlazorServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Ports;

namespace HygroclipBlazorServer.Devices
{
    public class HygroclipControllerModel
    {
        public event EventHandler<EnvironmentalMeasurement>? NewEnvironmentalMeasurement;
        public event EventHandler<string[]>? ComPortListUpdated;

        public HygroclipControllerModel()
        {
            UpdateComPortList();
            SelectedSerialPort = SerialPorts.LastOrDefault();
        }

        public double Tempearture { get; private set; } = double.NaN;
        public double Humidity { get; private set; } = double.NaN;
        public string[] SerialPorts { get; protected set; } = Array.Empty<string>();
        public double PollingInterval
        {
            get => _hygroclip?.PollingInterval ?? double.NaN;
            set
            {
                _cachedPollingInterval = value;
                if (_hygroclip is not null) _hygroclip.PollingInterval = value;
            }
        }
        private double _cachedPollingInterval = 2000;

        public string? SelectedSerialPort
        {
            get => _selectedSerialPort;
            set
            {
                _selectedSerialPort = value;
                if (value is not null) CreateHygroclipDevice(value);
            }
        }
        private string? _selectedSerialPort;

        internal virtual void UpdateComPortList()
        {
            SerialPorts = SerialPort.GetPortNames();
            ComPortListUpdated?.Invoke(this, SerialPorts);
        }

        protected virtual void CreateHygroclipDevice(string comPortName)
        {
            InitializeHygroclipDevice(new HygroclipDevice(comPortName)
            {
                PollingInterval = _cachedPollingInterval
            });
        }

        protected void InitializeHygroclipDevice(HygroclipDevice device)
        {
            if (_hygroclip is not null)
            {
                _hygroclip.Halt();
                _hygroclip.NewMeasurement -= Hygroclip_NewMeasurement;

                Tempearture = double.NaN;
                Humidity = double.NaN;
            }

            _hygroclip = device;
            _hygroclip.NewMeasurement += Hygroclip_NewMeasurement;
        }

        private void Hygroclip_NewMeasurement(object? sender, HygroclipDriver.HygroclipDriver.HygroClipMeasurment measurment)
        {
            Tempearture = measurment.Temperature;
            Humidity = measurment.Humidity;

            NewEnvironmentalMeasurement?.Invoke(this, new EnvironmentalMeasurement()
            {
                DateTime = DateTime.Now,
                Temperature = Tempearture,
                Humidity = Humidity
            });
        }

        private HygroclipDevice? _hygroclip;

    }
}
