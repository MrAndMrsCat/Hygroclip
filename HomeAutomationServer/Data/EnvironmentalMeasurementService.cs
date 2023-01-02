using Microsoft.EntityFrameworkCore;

namespace HomeAutomationServer.Data
{
    public class EnvironmentalMeasurementService
    {
        public int MaximumMeasurementCount { get; set; } = 1000;

        public EnvironmentalMeasurementService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            _measurementCache[TimeSpan.FromMinutes(65)] = new();
            _measurementCache[TimeSpan.FromHours(7)] = new();
            _measurementCache[TimeSpan.FromHours(36)] = new();

            PopulateCacheAsync();

            Program.HardwareModel.HygroclipController!.NewEnvironmentalMeasurement += async (s, meas) =>
            {
                foreach (var (span, cachedMeas) in _measurementCache.Select(kv => (kv.Key, kv.Value)))
                {
                    TimeSpan minimumInterval = span / MaximumMeasurementCount;

                    if (cachedMeas.Any() && cachedMeas.Last().DateTime + minimumInterval < meas.DateTime)
                    {
                        cachedMeas.Add(meas);
                    }

                    // discard old measurements
                    while (cachedMeas.Any() && cachedMeas[0].DateTime + span < meas.DateTime) cachedMeas.RemoveAt(0);
                }

                // add to database
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<EnvironmentalMeasurementContext>();

                if (context.MeasurementData is not null)
                {
                    context.MeasurementData.Add(new()
                    {
                        DateTime = DateTime.Now,
                        Temperature = meas.Temperature,
                        Humidity = meas.Humidity,
                    });

                    await context.SaveChangesAsync();

                    DatabaseUpdated?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public event EventHandler? DatabaseUpdated;
        
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly Dictionary<TimeSpan, List<EnvironmentalMeasurement>> _measurementCache = new();

        private async void PopulateCacheAsync()
        {
            await Task.Run(() =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<EnvironmentalMeasurementContext>();

                if (context.MeasurementData is not null)
                {
                    foreach (TimeSpan span in _measurementCache.Keys)
                    {
                        List<EnvironmentalMeasurement> measInRange = new();

                        foreach (EnvironmentalMeasurement meas in context.MeasurementData)
                        {
                            if (meas.DateTime > DateTime.Now - span)
                            {
                                measInRange.Add(new()
                                {
                                    DateTime = meas.DateTime.ToLocalTime(),
                                    Temperature = meas.Temperature,
                                    Humidity = meas.Humidity,
                                });
                            }
                        }

                        measInRange = measInRange.OrderBy(m => m.DateTime).ToList();
                        TimeSpan minimumInterval = span / MaximumMeasurementCount;

                        List<EnvironmentalMeasurement> measWithMinimumInterval = new();
                        DateTime lastMeas = DateTime.MinValue;

                        foreach (var meas in measInRange)
                        {
                            if (meas.DateTime > lastMeas + minimumInterval)
                            {
                                lastMeas = meas.DateTime;
                                measWithMinimumInterval.Add(meas);
                            }
                        }

                        _measurementCache[span] = measWithMinimumInterval;
                    }
                }
            });
        }

        public Task<EnvironmentalMeasurement[]?> GetMeasurementsAsync(DateTime startDate, int count = 0)
        {
            return Task.Run(() =>
            {
                EnvironmentalMeasurement[]? measurements = null;

                foreach (var (span, cachedMeas) in _measurementCache.Select(kv => (kv.Key, kv.Value)))
                {
                    if (startDate + span > DateTime.Now)
                    {
                        measurements = cachedMeas.ToArray();
                    }
                }

                if (measurements is null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    measurements = scope.ServiceProvider.GetRequiredService<EnvironmentalMeasurementContext>().MeasurementData?.ToArray();
                }

                var allMeas = measurements?.Where(meas => meas.DateTime > startDate);

                return count > 0
                    ? allMeas?.TakeLast(count).ToArray()
                    : allMeas?.ToArray();
            });
        }

        public Task<EnvironmentalMeasurement[]?> GetRecentMeasurementsAsync(int measurementCount, TimeSpan minimumInterval)
        {
            return Task.Run(() =>
            {
                using var scope = _scopeFactory.CreateScope();
                var measurementData = scope.ServiceProvider.GetRequiredService<EnvironmentalMeasurementContext>().MeasurementData;

                if (measurementData is not null)
                {
                    List<EnvironmentalMeasurement>? result = new();

                    var recent = measurementData.OrderByDescending(meas => meas.DateTime);

                    DateTime lastTime = DateTime.MaxValue;
                    int count = 0;

                    foreach (var meas in recent)
                    {
                        if (meas.DateTime + minimumInterval < lastTime)
                        {
                            result.Add(meas);
                            lastTime = meas.DateTime;
                            count++;
                            if (count >= measurementCount) break;
                        }
                    }

                    return result?.ToArray();
                }
                else return null;
            });
        }
    }


}
