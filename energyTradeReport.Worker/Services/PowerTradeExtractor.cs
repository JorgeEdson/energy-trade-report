using Axpo;
using energyTradeReport.Worker.Settings;
using Microsoft.Extensions.Options;
using TimeZoneConverter;

namespace energyTradeReport.Worker.Services
{
    public class PowerTradeExtractor
    {
        private readonly PowerReportSettings _settings;
        private readonly ILogger<PowerTradeExtractor> _logger;

        public PowerTradeExtractor(IOptions<PowerReportSettings> options, ILogger<PowerTradeExtractor> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<Dictionary<TimeSpan, double>> GetAggregatedVolumeByHourAsync(DateTime referenceDateUtc)
        {
            try
            {
                var powerService = new PowerService();
                var trades = await powerService.GetTradesAsync(referenceDateUtc.Date);

                var aggregated = new Dictionary<TimeSpan, double>();

                foreach (var trade in trades)
                {
                    foreach (var period in trade.Periods)
                    {
                        var localTime = GetLocalTimeFromPeriod(period.Period, referenceDateUtc.Date);
                        if (aggregated.ContainsKey(localTime))
                            aggregated[localTime] += period.Volume;
                        else
                            aggregated[localTime] = period.Volume;
                    }
                }

                _logger.LogInformation("Trades agregados com sucesso para o dia {date}", referenceDateUtc.Date);
                return aggregated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair dados do PowerService");
                throw;
            }
        }



        private TimeSpan GetLocalTimeFromPeriod(int period, DateTime day)
        {
            // Cada período representa uma hora, começando às 23:00 do dia anterior
            var londonZone = TZConvert.GetTimeZoneInfo(_settings.TimeZone); // "GMT Standard Time"
            var startTime = day.AddDays(-1).Date.AddHours(23).AddHours(period - 1);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(startTime.ToUniversalTime(), londonZone);
            return localTime.TimeOfDay;
        }
    }
}
