using energyTradeReport.Worker.Services;
using energyTradeReport.Worker.Settings;
using Microsoft.Extensions.Options;

namespace energyTradeReport.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly PowerReportSettings _settings;
        private readonly CsvExporter _csvExporter;
        private readonly PowerTradeExtractor _extractor;
        private readonly PowerAggregatorService _aggregator;

        public Worker(
            ILogger<Worker> logger, 
            IOptions<PowerReportSettings> options,
            CsvExporter csvExporter,
            PowerTradeExtractor extractor,
            PowerAggregatorService aggregator)
        {
            _logger = logger;
            _settings = options.Value;
            _csvExporter = csvExporter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Relat�rio iniciado");
            _logger.LogInformation("Diret�rio de sa�da: {dir}", _settings.OutputDirectory);
            _logger.LogInformation("Intervalo de execu��o: {min} minutos", _settings.IntervalMinutes);

            // Executa a primeira extra��o imediatamente
            await ExecuteReportCycleAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(_settings.IntervalMinutes), stoppingToken);

                await ExecuteReportCycleAsync(stoppingToken);
            }
        }

        private async Task ExecuteReportCycleAsync(CancellationToken stoppingToken)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                var localNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcNow, _settings.TimeZone);
                _logger.LogInformation("Executando relat�rio �s {hora}", localNow);

                // 1. Extrair dados brutos da DLL
                var rawTrades = await _extractor.GetAggregatedVolumeByHourAsync(utcNow);

                // 2. Agregar usando dom�nio rico
                var dailyPosition = _aggregator.Aggregate(rawTrades, DateOnly.FromDateTime(utcNow));

                if (dailyPosition == null)
                {
                    _logger.LogWarning("DailyPowerPosition n�o p�de ser criada.");
                    return;
                }

                var volumes = dailyPosition.GetAggregatedVolumes();
                var filePath = _csvExporter.Export(volumes);

                _logger.LogInformation("Relat�rio exportado com sucesso: {arquivo}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar o relat�rio");
            }
        }
    }
}
