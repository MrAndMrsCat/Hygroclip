using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace HygroclipDriver
{
    public class SerialPortDevice : ISerialComms
    {
        public class PortConfiguration
        {
            public StopBits StopBits { get; init; } = StopBits.One;
            public Parity Parity { get; init; } = Parity.None;
            public string PortName { get; init; } = "COM1";
            public int BaudRate { get; init; } = 9600;
            public int DataBits { get; init; } = 8;
            public char? TerminationCharacter { get; init; }
        }

        public SerialPortDevice(PortConfiguration config)
        {
            _serialPort = new()
            {
                StopBits = config.StopBits,
                Parity = config.Parity,
                PortName = config.PortName,
                BaudRate = config.BaudRate,
                DataBits = config.DataBits
            };

            _serialPort.DataReceived += (s, e) =>
            {
                try
                {
                    byte[] bytes = new byte[_serialPort.BytesToRead];

                    _serialPort.Read(bytes, 0, bytes.Length);

                    if (config.TerminationCharacter is null)
                    {
                        ReceivedBytes?.Invoke(s, bytes);
                    }
                    else
                    {
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            if (bytes[i] == config.TerminationCharacter)
                            {
                                ReceivedBytes?.Invoke(s, _buffer.ToArray());
                                _buffer.Clear();
                            }
                            else
                            {
                                _buffer.Add(bytes[i]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.ForContext<SerialPortDevice>().Error(ex.Message);
                }
            };
        }

        private readonly SerialPort _serialPort;
        private readonly List<byte> _buffer = new();

        public event EventHandler<byte[]>? ReceivedBytes;

        public void Send(byte[] bytes)
        {
            try
            {
                if (!_serialPort.IsOpen) _serialPort.Open();

                _serialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Serilog.Log.ForContext<SerialPortDevice>().Error(ex.Message);
            }
        }

        public void Halt()
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}
