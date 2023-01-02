using HygroclipDriver;

namespace HomeAutomationServer.Devices
{
    public class SimulatedHygroclipControllerModel : HygroclipControllerModel
    {

        internal override void UpdateComPortList()
        {
            SerialPorts = new string[] { "COM5", "COM7", "COM8" };
        }

        protected override void CreateHygroclipDevice(string comPortName)
        {
            InitializeHygroclipDevice(new HygroclipDevice());
        }
    }
}
