using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shirehorse.Core.FiniteStateMachines;

namespace PicoController;

public class PicoBoilerTemperatureControlPolicy
{
    public PicoBoilerTemperatureControlPolicy(PicoBoilerInterface boilerInterface)
    {
        _boilerInterface = boilerInterface;
    }


    public enum State
    {
        Disabled,
        Controlling,
        DelayingActuation,
    }

    public event EventHandler<State>? StateChanged;

    private readonly FiniteStateMachine<State> _fsm = new();

    public bool ControlEnabled
    {
        get => _fsm.CurrentState != State.Disabled;
        set => _fsm.ChangeState(value ? State.Controlling : State.Disabled);
    }
    public State ControlState => _fsm.CurrentState;
    public float TemperatureSetpoint { get; set; } = 13f;
    public float ThermostatDeadband { get; set; } = 0.1f;
    public float ActuatorThrottleTimeMinutes { get; set; } = 2f;
    public double TemporaryBoostTimeMinutes
    {
        get => TimeSpan.FromMilliseconds(_boostTimer.Interval).TotalMinutes;
        set => _boostTimer.Interval = TimeSpan.FromMinutes(value).TotalMilliseconds;
    }

    private readonly System.Timers.Timer _boostTimer = new();

    private readonly PicoBoilerInterface _boilerInterface;

    public void Initialize()
    {
        BuildFSM();

        _boilerInterface.ConnectionChanged += (s, connected) =>
        {
            if (connected)
            {
                _boilerInterface.SetTime();
                _boilerInterface.RequestStatus();
            }
        };

        _boilerInterface.StatusReceived += (s, status) =>
        {
            if (_boilerInterface.DeviceState == PicoBoilerInterface.State.Uninitialized)
            {
                _boilerInterface.InitializeDevice();
            }

            if (status.TryGetValue("temperature_setpoint", out string? setpoint))
            {
                if (float.TryParse(setpoint, out float temperature))
                {
                    if (TemperatureSetpoint != temperature) _boilerInterface.SendTemperatureSetpoint(TemperatureSetpoint);
                }
            }
        };
    }

    private void BuildFSM()
    {
        // Transitions
        _fsm[State.Disabled, State.Controlling] = new();
        _fsm[State.Controlling, State.DelayingActuation] = new();
        _fsm[State.Controlling, State.Disabled] = new();
        _fsm[State.DelayingActuation, State.Controlling] = new();
        _fsm[State.DelayingActuation, State.Disabled] = new();

        // Actions
        _fsm[State.Disabled].EntryAction = () =>
        {
            _boilerInterface.BoilerEnabled = false;
            _boilerInterface.ControlEnabledStatus = false;
        };

        _fsm[State.Controlling] = new() 
        {
            EntryAction= () => _boilerInterface.ControlEnabledStatus = true,

            // prevent frequent actuation, could damage boiler..
            PollingInterval = 5000,
            PollingCompleteState = State.DelayingActuation,
            PollingFunction = () =>
            {
                if (!_boilerInterface.Connected) return false;

                bool actuated = false;

                if (_boilerInterface.Temperature > TemperatureSetpoint + ThermostatDeadband)
                {
                    if (_boilerInterface.BoilerEnabled)
                    {
                        _boilerInterface.BoilerEnabled = false;
                        actuated = true;
                    }
                   
                }
                else if (_boilerInterface.Temperature < TemperatureSetpoint - ThermostatDeadband)
                {
                    if (!_boilerInterface.BoilerEnabled)
                    {
                        _boilerInterface.BoilerEnabled = true;
                        actuated = true;
                    }
                }

                return actuated;
            }
        };

        _fsm[State.DelayingActuation] = new(timeout: (int)(ActuatorThrottleTimeMinutes * 60 * 1000), timeoutState: State.Controlling);

        _fsm.StateChanged += (s, e) =>
        {
            Logger.Debug($"ControlPoliscyFSM state={e.NewState}");
            StateChanged?.Invoke(this, e.NewState);
        };
    }
}
