using energyTradeReport.Domain.Helpers;

namespace energyTradeReport.Domain.Entities
{
    public class PowerTrade
    {
        public List<PowerPeriod> PowerPeriods { get; private set; }

        private void SetPowerPeriods(List<PowerPeriod> periods) 
        {
            if (periods == null || !periods.Any())
                throw new ArgumentOutOfRangeException(nameof(periods), "Lista de PowerPeriods vazia");

            PowerPeriods = periods;
        }
        private PowerTrade(List<PowerPeriod> periods)
        {
            SetPowerPeriods(periods);
        }

        public static GenericResult<PowerTrade> Instance(List<PowerPeriod> periods)
        {
            try
            {
                return new GenericResult<PowerTrade>(
                  true,
                  "PowerTrade criado com sucesso",
                  new PowerTrade(periods)
                );
            }
            catch (Exception ex)
            {
                return new GenericResult<PowerTrade>(
                    false,
                    ex.Message,
                    null
                );

            }
        }
    }
}
