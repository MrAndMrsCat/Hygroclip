namespace HomeAutomationServer.Devices;

public class SystemDevicesModel
{
    public HygroclipControllerModel? HygroclipController { get; private set; }
    public PicoBoilerControllerModel PicoBoilerController { get; } = new();

    public void Initialize(bool simulated = false)
    {
        HygroclipController = simulated ? new SimulatedHygroclipControllerModel() : new HygroclipControllerModel();
        HygroclipController.PollingInterval = 2000;

        PicoBoilerController.Initialize();

        HygroclipController.NewEnvironmentalMeasurement += (s, meas) =>
        {
            PicoController.Logger.Debug($"send temp={meas.Temperature}, humidity={meas.Humidity}");
            PicoBoilerController.SendEnvironmentalMeasuremnt((float)meas.Temperature, (float)meas.Humidity);
        };
    }
}
