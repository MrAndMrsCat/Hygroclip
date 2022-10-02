using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HygroclipDriver
{
    internal class SimulatedHygroclipSerialPort : ISerialComms
    {
        public event EventHandler<byte[]>? ReceivedBytes;

        public int MessageProcessingDelayMilliseconds { get; set; } = 20;

        public double Tempearature { get; private set; } = DefaultTemperature; // degC
        public double Humidity { get; private set; } = DefaultHumidity; // %

        private const double DefaultTemperature = 21;
        private const double DefaultHumidity = 60;

        private double RandomWalkDistance { get; set; } = 2;
        private readonly Random _randomGenerator = new();

        public void Halt() { } // do nothing

        private double NewReading(double currentValue, double defaultValue, double walkDistance)
        {
            double currentAbsDistanceNormalized = Math.Abs(currentValue - defaultValue) / walkDistance;

            double probablityToWalkBackToDefault = Math.Min(1, 1 - currentAbsDistanceNormalized);

            int sign = probablityToWalkBackToDefault - 0.5 > 0 ? -1 : 1;

            double newDistance = _randomGenerator.NextDouble() * RandomWalkDistance;

            return currentValue + sign * newDistance;
        }

        public void Send(byte[] bytes)
        {
            Task.Delay(MessageProcessingDelayMilliseconds)
                .GetAwaiter()
                .OnCompleted(() => 
                {
                    string message = Encoding.ASCII.GetString(bytes);

                    Tempearature = NewReading(Tempearature, DefaultTemperature, RandomWalkDistance);
                    Humidity = NewReading(Humidity, DefaultHumidity, RandomWalkDistance);

                    string reply = message switch
                    {
                        "{ 99RDD}\r" => $"{{F00rdd 001; {Humidity:N2};%rh;000;-; {Tempearature:N2};?C;000;=;nc;---.- ;?C;000; ;001;V3.2-1;0020105749;HC2         ;000;F",
                        _ => "Does not understand",
                    };

                    ReceivedBytes?.Invoke(this, Encoding.ASCII.GetBytes(reply));
                });
        }
    }
}
