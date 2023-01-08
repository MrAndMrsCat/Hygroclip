using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shirehorse.Core.Extensions;

namespace PicoController
{
    public class PicoBoilerInterface
    {
        public enum State
        {
            Uninitialized,
            Initializing,
            Idle,
            ActuatingBoiler
        }

        public bool Connected => _picoClient?.Connected ?? false;
        public State? DeviceState => Connected ? _deviceState : null;
        private State _deviceState;
        public bool BoilerEnabled
        {
            get => _boilerEnabled;
            set
            {
                _boilerEnabled = value;
                if (_boilerEnabledInput != value)
                {
                    SetBoilerEnabled(value);
                }
            }
        }
        private bool _boilerEnabled;
        private bool? _boilerEnabledInput;
        public float Temperature => _temperature;
        private float _temperature = float.NaN;
        public float Humidity => _humidity;
        private float _humidity = float.NaN;

        public bool ControlEnabledStatus { get; set; }

        public SortedDictionary<string, string>? StatusParameters { get; private set; }

        public event EventHandler<Dictionary<string, string>>? StatusReceived;
        public event EventHandler<bool>? ConnectionChanged;

        private PicoTCPClient? _picoClient;
        private PicoTCPConnectionPolicy? _connectionPolicy;
        public PicoBoilerTemperatureControlPolicy? ControlPolicy { get; private set; }
        public PicoBoilerTimerControlPolicy? TimerPolicy { get; private set; }

        public void Initiaize()
        {
            InitializeTCPClient();
            InitializePolicies();
        }

        private void InitializeTCPClient()
        {
            if (_picoClient is not null) throw new InvalidOperationException("Already initialized");

            _picoClient = new()
            {
                MessageTerminationByte = 4, 
            };

            _picoClient.ConnectionChanged += (s, connected) =>
            {
                ConnectionChanged?.SafeInvoke(s, connected);
                if (!connected) _boilerEnabledInput = null;
            };

            _picoClient.BytesReceived += (s, bytes) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(bytes);
                    Logger.Info($"Rx << {message}");

                    string[] msg_args = message.Split('|');
                    Dictionary<string, string> parameters = new();

                    if (msg_args.Length == 2)
                    {
                        string[] parametersKeysValues = msg_args[1].Split(',');
                        foreach (var pair in parametersKeysValues)
                        {
                            var keyValue = pair
                                .Split("=")
                                .Select(token => token.Trim())
                                .ToList();

                            if (keyValue.Count == 2)
                            {
                                parameters[keyValue[0]] = keyValue[1];
                            }
                        }
                    }

                    switch (msg_args[0])
                    {
                        case "status":
                            foreach (var (key, value) in parameters)
                            {
                                try
                                {
                                    switch (key)
                                    {
                                        case "state":
                                            _deviceState = Enum.Parse<State>(value);
                                            break;

                                        case "boiler_enabled":
                                            _boilerEnabledInput = bool.Parse(value);
                                            if (_boilerEnabled != _boilerEnabledInput && _deviceState != State.ActuatingBoiler)
                                            {
                                                SetBoilerEnabled(_boilerEnabled);
                                            }
                                            break;

                                        case "temperature":
                                            _temperature = float.Parse(value);
                                            break;

                                        case "humidity":
                                            _humidity = float.Parse(value);
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"failed to parse {key}={value}: {ex.Message}");
                                    Logger.Debug(ex);
                                }
                            }

                            Logger.Debug($"parsed {parameters.Count} parameters");
                            StatusParameters = new(parameters);
                            StatusReceived?.SafeInvoke(this, parameters);
                            break;

                        case "ack":
                            if (msg_args[1] == "set_parameters") RequestStatus();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"failed to parse message: {ex.Message}");
                    Logger.Debug(ex);
                }
            };
        }

        private void InitializePolicies()
        {
            ControlPolicy = new(this);
            ControlPolicy.Initialize();

            TimerPolicy = new(this);
            TimerPolicy.Initialize();

            _connectionPolicy = new(_picoClient!);
            _connectionPolicy.Initialize();
        }

        public void SendMessage(string message)
        {
            if (_picoClient is null)
            {
                Logger.Error("cannot send message, client is not initialized");
            }
            else
            {
                Logger.Info($"Tx >> {message}");
                _picoClient.Send(Encoding.UTF8.GetBytes(message));
            }

        }

        public void SendParameters(Dictionary<string, object> parameters)
        {
            var paramss = parameters.Select(kv => $"{kv.Key}={kv.Value}");

            SendMessage("set_parameters|" + string.Join(",", paramss));
        }

        public void RequestStatus() => SendMessage("req_status");
        public void InitializeDevice() => SendMessage("initialize");
        public void SetBoilerEnabled(bool enabled)
        {
            SendParameters(new()
            {
                { $"boiler_enabled", enabled },
                { $"control_enabled", ControlEnabledStatus },
            });
        }
        public void SendMeasurment(float temperature, float humidity)
        {
            SendParameters(new() 
            { 
                { "temperature", temperature },
                { "humidity", humidity },
            });
        }
        public void SendControlStatus(bool enabled) => SendParameters(new() { { $"control_enabled", enabled } });
        public void SendTemperatureSetpoint(float temperature) => SendParameters(new() { { $"temperature_setpoint", temperature } });
        public void SetTime()
        {
            // python RTC uses (year, month, day, weekday, hours, minutes, seconds, subseconds)
            DateTime now = DateTime.Now;
            int day = _dayMap[$"{now:ddd}"];
            SendParameters(new() { { $"datetime", $"{now:yyyy MM dd} {day} {now:HH mm ss} 0" } });
        }

        private readonly Dictionary<string, int> _dayMap = new()
        {
            {"Mon", 0 },
            {"Tue", 1 },
            {"Wed", 2 },
            {"Thu", 3 },
            {"Fri", 4 },
            {"Sat", 5 },
            {"Sun", 6 },
        };
    }
}
