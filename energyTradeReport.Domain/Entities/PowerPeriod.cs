using energyTradeReport.Domain.Helpers;

namespace energyTradeReport.Domain.Entities
{
    public class PowerPeriod
    {
        public int Period { get; private set; }
        public double Volume { get; private set; }
        


        private void SetPeriod(int periodParam) 
        {
            if (periodParam < 1 || periodParam > 24)
                throw new ArgumentOutOfRangeException(nameof(periodParam), "Período deve estar entre 1 e 24");

            Period = periodParam;
        }

        private void SetVolume(double volumeParam)
        {
            if (volumeParam <= 0 )
                throw new ArgumentOutOfRangeException(nameof(volumeParam), "O Volume nao pode ser 0 ou negativo");

            Volume = volumeParam;
        }


        private PowerPeriod(int periodParam, double volumeParam)
        {
            SetPeriod(periodParam);
            SetVolume(volumeParam);            
        }

        public static GenericResult<PowerPeriod> Instance(int periodParam, double volumeParam)
        {
            try
            {
                return new GenericResult<PowerPeriod>(
                  true,
                  "PowerPeriod criado com sucesso",
                  new PowerPeriod(periodParam,volumeParam)
                );

            }
            catch (Exception ex) 
            {
                return new GenericResult<PowerPeriod>(
                    false,
                    ex.Message,
                    null
                );            
            }
        }
    }
}
