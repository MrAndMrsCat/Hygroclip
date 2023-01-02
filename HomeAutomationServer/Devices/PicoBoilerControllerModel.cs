using PicoController;
using HomeAutomationServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;

namespace HomeAutomationServer.Devices;

public class PicoBoilerControllerModel
{
    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<Dictionary<string,string>>? StatusReceived;

    public bool Connected => _boilerController.Connected;
    public bool BoilerEnabled
    {
        get => _boilerController.BoilerEnabled;
        set => _boilerController.BoilerEnabled = value;
    }

    public float TemperatureSetpoint
    {
        get => ControlPolicy?.TemperatureSetpoint ?? float.NaN;
        set
        {
            if (ControlPolicy is not null) ControlPolicy.TemperatureSetpoint = value;
        }
    }

    public PicoBoilerTemperatureControlPolicy? ControlPolicy => _boilerController.ControlPolicy;
    public PicoBoilerTimerControlPolicy? TimerPolicy => _boilerController.TimerPolicy;

    public PicoBoilerTimerControlPolicy.ControlMode Mode
    {
        get => TimerPolicy?.Mode ?? PicoBoilerTimerControlPolicy.ControlMode.OFF;
        set
        {
            if (TimerPolicy is not null) TimerPolicy.Mode = value;
        }
    }

    public bool BoostEnabled
    {
        get => TimerPolicy?.BoostEnabled ?? false;
    }

    public void Boost() => TimerPolicy?.Boost();

    public SortedDictionary<string, string>? StatusParameters => _boilerController.StatusParameters;

    private readonly PicoBoilerInterface _boilerController = new();
    private readonly System.Timers.Timer _pollingTimer = new(10000);

    internal void Initialize()
    {
        _boilerController.ConnectionChanged += (s, connected) => ConnectionChanged?.Invoke(s, connected);
        _boilerController.StatusReceived += (s, parameters) =>
        {
            _pollingTimer.Stop();
            _pollingTimer.Start();
            StatusReceived?.Invoke(s, parameters);
        };

        _boilerController.Initiaize();

        _pollingTimer.Elapsed += (s, e) => _boilerController.RequestStatus();
        _pollingTimer.Start();

        Mode = PicoBoilerTimerControlPolicy.ControlMode.Timed;
    }

    internal void SendEnvironmentalMeasuremnt(float tempearure, float humidity)
    {
        _boilerController.SendMeasurment(tempearure, humidity);
    }
}
