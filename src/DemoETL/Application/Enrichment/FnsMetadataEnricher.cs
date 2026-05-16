using DemoETL.Application.Enrichment.Interfaces;
using DemoETL.Domain.Models;
using Microsoft.Extensions.Logging;


namespace DemoETL.Application.Enrichment;

/// <summary>
/// Для демонстрации.
/// Заглушка для enrichment сервиса.
///
/// Выполняет enrichment модели:
/// - дополняет отсутствующие данные;
/// - вычисляет что-нибудь;
/// - подготавливает модель
///   к трансформации.
///
/// В MVP:
/// - имитирует получение КПП по ИНН.
///
/// В проде возможно:
/// - запросы к ЕГРЮЛ;
/// - запросы к ФНС API;
/// - интеграция с внешними сервисами;
/// - справочники.
///
/// Это отдельный этап pipeline.
///
/// Это позволяет:
/// - не смешивать extraction и интеграцию с сервисами;
/// - не смешивать маппинг и внешние IO;
/// - проще расширять проект.
/// </summary>
public class FnsMetadataEnricher : IDataEnricher
{
    private readonly ILogger<FnsMetadataEnricher> _logger;

    /// <summary>
    /// Основной конструктор
    /// </summary>
    /// <param name="logger"></param>
    public FnsMetadataEnricher(ILogger<FnsMetadataEnricher> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Логика наполнения модели данными, которых нет в инпуте
    /// </summary>
    /// <param name="model">Модель для наполнения</param>
    /// <returns></returns>
    public Task EnrichAsync(ImportDeclaration model)
    {
        _logger.LogInformation("Model enrichment started");

        if (string.IsNullOrWhiteSpace(model.Inn))
        {
            _logger.LogWarning(
                "INN is empty, enrichment skipped");

            return Task.CompletedTask;
        }

        if (model.Inn == "7802579868")
        {
            model.Kpp = "780201001";

            model.TaxAuthority = "7802";

            _logger.LogInformation(
                "KPP enriched: {Kpp}",
                model.Kpp);

            _logger.LogInformation(
                "TaxAuthority enriched: {TaxAuthority}",
                model.TaxAuthority);
        }
        else
        {
            _logger.LogWarning(
                "No KPP mapping found for INN: {Inn}",
                model.Inn);
        }

        return Task.CompletedTask;
    }
}
