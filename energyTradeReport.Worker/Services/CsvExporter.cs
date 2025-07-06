using energyTradeReport.Worker.Settings;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace energyTradeReport.Worker.Services
{
    public class CsvExporter
    {
        private readonly string _outputDir;
        private readonly ILogger<CsvExporter> _logger;


        public CsvExporter(IOptions<PowerReportSettings> options, ILogger<CsvExporter> logger)
        {
            _outputDir = options.Value.OutputDirectory ?? "./output";
            _logger = logger;
        }

        public string Export(IReadOnlyDictionary<TimeSpan, double> aggregatedVolumes)
        {
            try
            {
                Directory.CreateDirectory(_outputDir); // Garante que o diretório exista

                var now = DateTime.Now;
                var fileName = $"PowerPosition_{now:yyyyMMdd}_{now:HHmm}.csv";
                var filePath = Path.Combine(_outputDir, fileName);

                var csv = new StringBuilder();
                csv.AppendLine("Local Time,Volume");

                foreach (var entry in aggregatedVolumes.OrderBy(e => e.Key))
                {
                    var timeFormatted = entry.Key.ToString(@"hh\:mm"); // Formato HH:mm
                    var volume = entry.Value.ToString("0.##", CultureInfo.InvariantCulture);
                    csv.AppendLine($"{timeFormatted},{volume}");
                }

                File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);

                _logger.LogInformation("Arquivo CSV exportado com sucesso: {path}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar arquivo CSV");
                throw;
            }
        }
    }
}
