using energyTradeReport.Domain.Helpers;

namespace energyTradeReport.Domain.Entities
{
    public class DailyPowerPosition
    {
        public DateOnly Date { get; private set; }

        private readonly Dictionary<TimeSpan, double> _aggregatedVolumes = new();

        private void SetDate(DateOnly dateParam) 
        {
            Date = Date;
        }

        private static TimeSpan PeriodToLocalTime(int period)
        {
            // Período 1 => 23:00 do dia anterior
            var hour = (period - 1 + 23) % 24;
            return TimeSpan.FromHours(hour);
        }

        private DailyPowerPosition(DateOnly dateParam)
        {
            SetDate(dateParam);                
        }

        public static GenericResult<DailyPowerPosition> Instance(DateOnly dateParam)
        {
            try
            {
                return new GenericResult<DailyPowerPosition>(
                  true,
                  "DailyPowerPosition criado com sucesso",
                  new DailyPowerPosition(dateParam)
                );

            }
            catch (Exception ex)
            {
                return new GenericResult<DailyPowerPosition>(
                    false,
                    ex.Message,
                    null
                );
            }
        }

        public IReadOnlyDictionary<TimeSpan, double> GetAggregatedVolumes() => _aggregatedVolumes;

        public void AddTrade(PowerTrade trade)
        {
            foreach (var period in trade.PowerPeriods)
            {
                var time = PeriodToLocalTime(period.Period);
                if (!_aggregatedVolumes.ContainsKey(time))
                    _aggregatedVolumes[time] = 0;

                _aggregatedVolumes[time] += period.Volume;
            }
        }
    }
}
