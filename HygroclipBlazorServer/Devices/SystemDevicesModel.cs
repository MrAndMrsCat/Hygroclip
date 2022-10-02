namespace HygroclipBlazorServer.Devices
{
    public class SystemDevicesModel
    {
        public HygroclipControllerModel? HygroclipController { get; set; }

        public void Initialize(bool simulated = false)
        {
            HygroclipController = simulated ? new SimulatedHygroclipControllerModel() : new HygroclipControllerModel();

            HygroclipController.PollingInterval = 2000;
        }
    }
}
