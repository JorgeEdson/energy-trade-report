using energyTradeReport.Worker;
using energyTradeReport.Worker.Services;
using energyTradeReport.Worker.Settings;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// L� config temporariamente para obter caminho
var tempConfig = builder.Configuration;
var outputDir = tempConfig.GetSection("PowerReportSettings")["OutputDirectory"] ?? "./output";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(tempConfig)    
    .WriteTo.File(
        path: Path.Combine(outputDir, "log.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7, // mant�m os �ltimos 7 arquivos de log
        shared: true)
    .CreateLogger();

builder.Logging.ClearProviders(); // remove o logger padr�o
builder.Logging.AddSerilog();

builder.Services.Configure<PowerReportSettings>(
    builder.Configuration.GetSection("PowerReportSettings"));

builder.Services.AddSingleton<CsvExporter>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
