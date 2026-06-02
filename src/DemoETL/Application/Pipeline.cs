using DemoETL.Application.Configuration;
using DemoETL.Application.Detection.Interfaces;
using DemoETL.Application.Enrichment.Interfaces;
using DemoETL.Application.Output;
using DemoETL.Domain.Models;
using DemoETL.Extraction;
using DemoETL.Extraction.Interfaces;
using DemoETL.Transformation.Builders;
using DemoETL.Transformation.Transformers;
using DemoETL.Validation.Interfaces;
using Microsoft.Extensions.Logging;


namespace DemoETL.Application;

/// <summary>
/// Оркестрация.
///
/// Pipeline координирует:
/// - extraction;
/// - parsing;
/// - mapping;
/// - enrichment;
/// - xml generation;
/// - xslt transformation;
/// - validation.
///
/// Важно:
/// Pipeline:
/// - не содержит extraction логики;
/// - не содержит mapping логики;
/// - не содержит xslt логики;
/// - не содержит xsd логики.
///
/// Pipeline только управляет потоком исполнения.
///
/// Использует:
/// - dependency injection;
/// - config-driven architecture;
/// - document metadata.
///
/// Должен поддерживать:
/// - разные типы документов (конфигурация);
/// - разные парсеры (WIP/TODO);
/// - разные трансформеры (если появится необходимость);
/// - разные валидаторы (если нужно).
///
/// Вход:
///     грязный XLS/XLSX с нулевым доверием
///
/// Выход:
///     валидный XML целевого формата.
/// </summary>
public class Pipeline
{
    private readonly ExcelReader _excelReader;
    private readonly IDocumentTypeDetector _detector;
    private readonly IEnumerable<IDocumentParser> _parsers;
    private readonly IEnumerable<IDataEnricher> _enrichers;
    private readonly IBusinessValidator _businessValidator;
    private readonly XmlBuilder _xmlBuilder;
    private readonly XsltTransformer _xsltTransformer;
    private readonly IXmlValidator _validator;
    private readonly AppConfig _config;
    private readonly ILogger<Pipeline> _logger;

    /// <summary>
    /// Собираем нужное пайплайну
    /// </summary>
    /// <param name="excelReader"></param>
    /// <param name="detector"></param>
    /// <param name="parsers"></param>
    /// <param name="enrichers"></param>
    /// <param name="businessValidator"></param>
    /// <param name="xmlBuilder"></param>
    /// <param name="xsltTransformer"></param>
    /// <param name="validator"></param>
    /// <param name="config"></param>
    /// <param name="logger"></param>
    public Pipeline(
        ExcelReader excelReader,
        IDocumentTypeDetector detector,
        IEnumerable<IDocumentParser> parsers,
        IEnumerable<IDataEnricher> enrichers,
        IBusinessValidator businessValidator,
        XmlBuilder xmlBuilder,
        XsltTransformer xsltTransformer,
        IXmlValidator validator,
        AppConfig config,
        ILogger<Pipeline> logger)
    {
        _excelReader = excelReader;
        _detector = detector;
        _parsers = parsers;
        _enrichers = enrichers;
        _businessValidator = businessValidator;
        _xmlBuilder = xmlBuilder;
        _xsltTransformer = xsltTransformer;
        _validator = validator;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Собственно Пайплайн
    /// </summary>
    /// <param name="options">Параметры из CLI (пути к файлам)</param>
    public async Task RunAsync(PipelineOptions options)
    {
        _logger.LogInformation("Pipeline started");

        // Проверяем наличие input
        if (!File.Exists(options.InputFile))
        {
            throw new FileNotFoundException(
                $"Input file not found: {options.InputFile}");
        }

        // Создаём output dir для конечных xml
        Directory.CreateDirectory("output");

        // Читаем raw эксель
        _logger.LogInformation("Reading Excel: {Path}", options.InputFile);

        var sheets = _excelReader.Read(options.InputFile);

        // Определяем тип дока
        var documentType = _detector.Detect(sheets);

        _logger.LogInformation("Detected document type: {DocumentType}", documentType);

        // Подбираем парсер по типу дока
        var parser = _parsers.FirstOrDefault(p => p.Type == documentType);

        if (parser == null)
            throw new Exception($"Parser not found for type {documentType}");

        // Парсим raw и перекладываем в нашу модель
        ImportDeclaration model = parser.Parse(sheets);

        _logger.LogInformation("Document parsed");

        /*
         * Enrichment: достаём данные, которых нет в исходных данных (например, КПП)
         * 
         * В данный момент от асинхронности нет пользы: энричер - заглушка, возвращать воркера в пул незачем.
         */
        foreach (var enricher in _enrichers)
        {
            await enricher.EnrichAsync(model);
        }

        _logger.LogInformation("Enrichment completed");

        // Контекст документа
        var documentContext =
            new DocumentContext
            {
                DateId = DateTime.Now.ToString("yyyyMMdd"),

                DateDoc = DateTime.Now.ToString("dd.MM.yyyy"),

                IdFileSuffix = Guid.NewGuid().ToString()
            };

        // Бизнес-валидация
        _businessValidator.Validate(model);

        _logger.LogInformation("Business validation completed");

        // Собираем наш нормализованный xml
        var intermediateXmlPath =
            options.KeepArtifacts
                ? Path.Combine(
                    options.DebugDirectory!,
                    $"{documentType}_" +
                    $"{DateTime.Now:yyyyMMdd_HHmmss}_" +
                    $"{Guid.NewGuid().ToString("N")[..6]}_" +
                    "intermediate.xml")
                : Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}.xml");

        if (options.KeepArtifacts)
        {
            Directory.CreateDirectory(
                options.DebugDirectory!);
        }

        var intermediateXml = _xmlBuilder.Build(model, documentContext);

        intermediateXml.Save(intermediateXmlPath);

        _logger.LogInformation("Intermediate XML saved: {Path}", intermediateXmlPath);

        // Резолвим метаданные по типу дока для трансформера и валидатора 
        if (!_config.Documents.TryGetValue(documentType, out var metadata))
            throw new Exception($"Metadata not found for type {documentType}");

        // Путь к файлу на выходе
        string finalXmlPath;

        if (!string.IsNullOrWhiteSpace(options.OutputFile))
        {
            finalXmlPath = options.OutputFile;
        }
        else
        {
            var fileName = FnsFileNameGenerator
                .GenerateCanonicalName(documentType, model, documentContext);

            finalXmlPath = Path.Combine(
                "output",
                $"{fileName}");
        }

        // XSLT
        var xsltPath = Path.Combine(
            AppContext.BaseDirectory,
            "Xslt",
            metadata.Xslt);

        _xsltTransformer.Transform(
            intermediateXmlPath,
            xsltPath,
            finalXmlPath,
            documentContext);

        _logger.LogInformation("Final XML generated: {Path}", finalXmlPath);

        // Валидируем по xsd
        var xsdPath = Path.Combine(
            AppContext.BaseDirectory,
            "Schemas",
            metadata.Xsd);

        _validator.Validate(finalXmlPath, xsdPath);

        _logger.LogInformation("XSD validation successful");

        // Чистка временных файлов
        if (!options.KeepArtifacts && File.Exists(intermediateXmlPath))
        {
           File.Delete(intermediateXmlPath);
        }

        _logger.LogInformation("Pipeline completed");

        await Task.CompletedTask;
    }
}
