using DemoETL.Application;
using DemoETL.Application.Configuration;
using DemoETL.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


/// <summary>
/// Точка входа приложения.
///
/// Ответственность:
/// - bootstrap приложения (лучше выносить в Infrastructure);
/// - инициализация DI (лучше выносить в коллекцию в Infrastructure - ServiceCollection Extension pattern);
/// - парсинг CLI аргументов;
/// - запуск ETL pipeline.
///
/// Pipeline:
///     XLS/XLSX
///         ↓
///     Extraction
///         ↓
///     Mapping
///         ↓
///     Enrichment
///         ↓
///     Intermediate XML
///         ↓
///     XSLT Transformation
///         ↓
///     Final XML
///         ↓
///     XSD Validation
///
/// Приложение намеренно реализовано как:
/// - консольный ETL MVP;
/// - config-driven;
/// - расширяемое (на сколько хватило времени).
///
/// Поддерживает:
/// - разные типы документов (благодаря выносу конфигурации);
/// - разные extraction-конфиги (вынесено в конфигурацию; пока только якоря вместо полноценного DSL);
/// - разные XSLT/XSD.
///
/// Пример запуска:
///
///     DemoETL.exe input.xlsx
///
///     DemoETL.exe input.xlsx -o result.xml
///     
///     DemoETL.exe input.xlsx -o c:\\exports\\result.xml
///
///     DemoETL.exe input.xlsx --debug-dir artifacts    # иначе промежуточный xml в %temp%, смотри пайплайн
///     
///     Используем -o, иначе дёргаем Transformation/Helpers/FnsFileNameGenerator и сохраняем в /output
///     
/// </summary>
internal class Program
{
    private static async Task Main(string[] args)
    {
        // для поддержки windows-1251; смотри Infrastructure/EncodingBootstrap
        // промежуточный xml в utf-8, выходной xml в windows-1251
        EncodingBootstrap.Register();

        var builder = Host.CreateApplicationBuilder(args);

        // грузим метаданные доков для оркестрации
        var appConfigPath = Path.Combine(
            AppContext.BaseDirectory,
            "Configs",
            "documents.json");
        var appConfig = AppConfig.Load(appConfigPath);

        builder.Services.AddSingleton(appConfig);

        // DI (смотри Infrastructure/ServiceCollectionExtensions)
        builder.Services.AddDemoEtl();

        var host = builder.Build();

        var pipeline = host.Services.GetRequiredService<Pipeline>();

        // CLI
        var options = CommandLineParser.Parse(args);

        await pipeline.RunAsync(options);
    }
}
