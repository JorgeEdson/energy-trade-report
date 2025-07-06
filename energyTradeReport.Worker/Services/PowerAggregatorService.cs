using energyTradeReport.Domain.Entities;

namespace energyTradeReport.Worker.Services
{
    public class PowerAggregatorService
    {
        private readonly ILogger<PowerAggregatorService> _logger;

        public PowerAggregatorService(ILogger<PowerAggregatorService> logger)
        {
            _logger = logger;
        }

        public DailyPowerPosition? Aggregate(List<PowerTrade> rawTrades, DateOnly date)
        {
            var positionResult = DailyPowerPosition.Instance(date);

            if (!positionResult.Sucess || positionResult.Object is null)
            {
                _logger.LogError("Erro ao criar DailyPowerPosition: {msg}", positionResult.Message);
                return null;
            }

            var dailyPosition = positionResult.Object;

            foreach (var rawTrade in rawTrades)
            {
                var powerPeriods = new List<PowerPeriod>();

                foreach (var rawPeriod in rawTrade.PowerPeriods)
                {
                    var periodResult = PowerPeriod.Instance(rawPeriod.Period, rawPeriod.Volume);
                    if (periodResult.Sucess && periodResult.Object is not null)
                    {
                        powerPeriods.Add(periodResult.Object);
                    }
                    else
                    {
                        _logger.LogWarning("PowerPeriod inválido (Período {period}, Volume {volume}): {msg}",
                            rawPeriod.Period, rawPeriod.Volume, periodResult.Message);
                    }
                }

                var tradeResult = PowerTrade.Instance(powerPeriods);

                if (tradeResult.Sucess && tradeResult.Object is not null)
                {
                    dailyPosition.AddTrade(tradeResult.Object);
                }
                else
                {
                    _logger.LogWarning("PowerTrade inválido: {msg}", tradeResult.Message);
                }
            }

            _logger.LogInformation("Agregação concluída para {date}", date);
            return dailyPosition;
        }
    }
}
