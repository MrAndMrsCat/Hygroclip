using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using HygroclipDriver;

namespace HygroclipDesktop
{
    internal class AppModel : AbstractViewModel
    {
        public AppModel()
        {
            UpdateComPortList();
            SelectedSerialPort = SerialPorts.LastOrDefault();
        }

        public double Tempearture
        {
            get => _temperature;
            set => SetValue(ref _temperature, value);
        }
        private double _temperature = double.NaN;

        public double Humidity
        {
            get => _humiditiy;
            set => SetValue(ref _humiditiy, value);
        }
        private double _humiditiy = double.NaN;

        public string[] SerialPorts
        {
            get => _serialPorts;
            set => SetValue(ref _serialPorts, value);
        }
        private string[] _serialPorts = Array.Empty<string>();

        public string? SelectedSerialPort
        {
            get => _selectedSerialPort;
            set
            {
                if (value is not null) CreateHygroclipDevice(value);

                SetValue(ref _selectedSerialPort, value);
            }
        }
        private string? _selectedSerialPort;

        private void UpdateComPortList()
        {
            SerialPorts = SerialPort.GetPortNames();
        }

        private void CreateHygroclipDevice(string comPortName)
        {
            if (_hygroclip is not null)
            {
                _hygroclip.Halt();
                _hygroclip.NewMeasurement -= Hygroclip_NewMeasurement;

                Tempearture = double.NaN;
                Humidity = double.NaN;
            }

            _hygroclip = new HygroclipDevice(comPortName);
            _hygroclip.NewMeasurement += Hygroclip_NewMeasurement;
        }

        private void Hygroclip_NewMeasurement(object? sender, HygroclipDriver.HygroclipDriver.HygroClipMeasurment measurment)
        {
            Tempearture = measurment.Temperature;
            Humidity = measurment.Humidity / 100;
        }

        private HygroclipDevice? _hygroclip;
    }
}
