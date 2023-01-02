using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicoController
{
    public class PicoBoilerTimerControlPolicy
    {
        public readonly record struct Setpoint(DayOfWeek Day, int Hour, int Min, bool Enabled);

        public PicoBoilerTimerControlPolicy(PicoBoilerInterface boilerInterface)
        {
            _boilerInterface = boilerInterface;
        }
        private readonly PicoBoilerInterface _boilerInterface;

        public enum ControlMode
        {
            OFF,
            ON,
            Timed
        }

        public ControlMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                BoostEnabled = false;
            }
        }
        private ControlMode _mode;

        public bool BoostEnabled
        {
            get => BoostEndDateTime is not null;
            set => BoostEndDateTime = value ? DateTime.Now + TimeSpan.FromMinutes(BoostLimitMinutes) : null;
        }

        public double BoostLimitMinutes { get; set; } = 60;
        public double BoostIntervalMinutes { get; set; } = 15;

        public DateTime? BoostEndDateTime { get; private set; }
        public double? BoostEndMinutes => (BoostEndDateTime - DateTime.Now)?.TotalMinutes;

        public void Boost()
        {
            if (BoostEndDateTime is null) BoostEndDateTime = DateTime.Now;

            BoostEndDateTime = BoostEndDateTime + TimeSpan.FromMinutes(BoostIntervalMinutes);

            if (BoostEndMinutes > BoostLimitMinutes) BoostEndDateTime = null;
        }

        public List<Setpoint> Setpoints { get; } = new() 
        {
            new Setpoint(DayOfWeek.Monday, 7, 30, true),
            new Setpoint(DayOfWeek.Monday, 21, 0, false),
            new Setpoint(DayOfWeek.Tuesday, 7, 30, true),
            new Setpoint(DayOfWeek.Tuesday, 8, 15, false),
            new Setpoint(DayOfWeek.Tuesday, 15, 30, true),
            new Setpoint(DayOfWeek.Tuesday, 21, 0, false),
            new Setpoint(DayOfWeek.Wednesday, 7, 30, true),
            new Setpoint(DayOfWeek.Wednesday, 21, 0, false),
            new Setpoint(DayOfWeek.Thursday, 7, 30, true),
            new Setpoint(DayOfWeek.Thursday, 8, 15, false),
            new Setpoint(DayOfWeek.Thursday, 15, 30, true),
            new Setpoint(DayOfWeek.Thursday, 21, 0, false),
            new Setpoint(DayOfWeek.Friday, 7, 30, true),
            new Setpoint(DayOfWeek.Friday, 21, 0, false),
            new Setpoint(DayOfWeek.Saturday, 7, 30, true),
            new Setpoint(DayOfWeek.Saturday, 21, 0, false),
            new Setpoint(DayOfWeek.Sunday, 7, 30, true),
            new Setpoint(DayOfWeek.Sunday, 21, 0, false),
        };
        public IEnumerable<Setpoint> OrderedSetpoints => Setpoints
            .OrderBy(sp => sp.Day)
            .ThenBy(sp => sp.Hour)
            .ThenBy(sp => sp.Min);

        private PicoBoilerTemperatureControlPolicy? TemperatureControlPolicy => _boilerInterface.ControlPolicy;

        private readonly System.Timers.Timer _evaluateSetpointsTimer = new(2000) { Enabled = true };

        public void Initialize()
        {
            _evaluateSetpointsTimer.Elapsed += (s, e) => EvaluateSetpoints();
        }



        private void EvaluateSetpoints()
        {
            if (BoostEndDateTime is not null)
            {
                if (DateTime.Now > BoostEndDateTime)
                {
                    BoostEndDateTime = null;
                }
                else
                {
                    _boilerInterface.BoilerEnabled = true;
                }
            }
            else if (TemperatureControlPolicy is not null)
            {
                switch (Mode)
                {
                    case ControlMode.ON:
                        TemperatureControlPolicy.ControlEnabled = true;
                        break;

                    case ControlMode.Timed 
                    when OrderedSetpoints.Any() 
                    && TemperatureControlPolicy.ControlState != PicoBoilerTemperatureControlPolicy.State.DelayingActuation:

                        Setpoint currentSetpoint = OrderedSetpoints.Last();
                        double nowHours = (int)DateTime.Now.DayOfWeek * 24 + DateTime.Now.Hour + DateTime.Now.Minute / 60.0;

                        foreach (var setpoint in OrderedSetpoints)
                        {
                            double setpointHours = (int)setpoint.Day * 24 + setpoint.Hour + setpoint.Min / 60.0;

                            if (setpointHours < nowHours) currentSetpoint = setpoint;
                        }

                        CurrentSetpoint = currentSetpoint;
                        TemperatureControlPolicy.ControlEnabled = currentSetpoint.Enabled;
                        break;

                    case ControlMode.OFF:
                        TemperatureControlPolicy.ControlEnabled = false;
                        _boilerInterface.BoilerEnabled = false;
                        break;
                }


            }
        }

        public Setpoint? CurrentSetpoint { get; private set; }
    }
}
