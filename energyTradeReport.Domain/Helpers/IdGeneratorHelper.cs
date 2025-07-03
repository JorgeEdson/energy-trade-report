namespace energyTradeReport.Domain.Helpers
{
    public class IdGeneratorHelper
    {
        private static long _lastTimestamp = -1L;
        private static int _counter = 0;
        private static readonly object _block = new();

        public static long NextId()
        {
            try
            {
                lock (_block)
                {
                    var timestamp = GetCurrentTimestamp();

                    if (timestamp == _lastTimestamp)
                    {
                        _counter++;
                    }
                    else
                    {
                        _counter = 0;
                        _lastTimestamp = timestamp;
                    }

                    return long.Parse($"{timestamp}{_counter:D4}");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar o próximo ID.", ex);
            }
        }

        private static long GetCurrentTimestamp()
        {
            return long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        }
    }
}
