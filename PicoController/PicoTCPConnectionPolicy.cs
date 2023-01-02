using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shirehorse.Core.FiniteStateMachines;

namespace PicoController
{
    internal class PicoTCPConnectionPolicy
    {
        public PicoTCPConnectionPolicy(PicoTCPClient tcpClient)
        {
            _tcpClient= tcpClient;
        }

        public enum State
        {
            Reset,
            Disconnected,
            Connecting,
            Connected,
        }

        public double ReconnectionDelaySeconds { get; set; } = 3;

        private readonly PicoTCPClient _tcpClient;

        private FiniteStateMachine<State>? _fsm;

        public void Initialize()
        {
            if (_fsm is not null) throw new InvalidOperationException("Already initialized");

            _fsm = new()
            {
                ResetAction = Start
            };

            // transitions
            _fsm[State.Reset, State.Connecting] = new();
            _fsm[State.Disconnected, State.Connecting] = new();
            _fsm[State.Connecting, State.Disconnected] = new();
            _fsm[State.Connecting, State.Connected] = new();
            _fsm[State.Connected, State.Disconnected] = new();

            // state actions
            _fsm[State.Disconnected] = new(timeout: (int)ReconnectionDelaySeconds * 1000, timeoutState: State.Connecting);
            _fsm[State.Connecting].EntryAction = () => _tcpClient.Connect();

            _tcpClient.ConnectionChanged += (s, connected) => _fsm.ChangeState(connected ? State.Connected : State.Disconnected);

            _fsm.StateChanged += (s, state) => Logger.Debug($"ConnectionPolicy - {state.NewState}");

            Start();
        }

        private void Start() => _fsm?.ChangeState(State.Connecting);
    }
}
