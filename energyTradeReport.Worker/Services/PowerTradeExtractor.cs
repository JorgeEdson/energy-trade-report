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

        public async Task<List<Domain.Entities.PowerTrade>> GetTradesFromServiceAsync(DateTime referenceDateUtc)
        {
            var powerService = new PowerService();
            var rawTrades = await powerService.GetTradesAsync(referenceDateUtc.Date);

            var domainTrades = new List<Domain.Entities.PowerTrade>();

            foreach (var rawTrade in rawTrades)
            {
                var domainPeriods = new List<Domain.Entities.PowerPeriod>();
                foreach (var rawPeriod in rawTrade.Periods)
                {
                    var periodResult = Domain.Entities.PowerPeriod.Instance(rawPeriod.Period, rawPeriod.Volume);
                    if (periodResult.Sucess && periodResult.Object is not null)
                        domainPeriods.Add(periodResult.Object);
                }

                var tradeResult = Domain.Entities.PowerTrade.Instance(domainPeriods);
                if (tradeResult.Sucess && tradeResult.Object is not null)
                    domainTrades.Add(tradeResult.Object);
            }

            return domainTrades;
        }
    }
}
