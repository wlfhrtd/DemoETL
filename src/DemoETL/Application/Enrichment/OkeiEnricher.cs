using DemoETL.Application.Enrichment.Interfaces;
using DemoETL.Domain.Models;
using Microsoft.Extensions.Logging;


namespace DemoETL.Application.Enrichment
{
    /// <summary>
    /// Для демонстрации.
    /// Заглушка для enrichment сервиса.
    /// Может использоваться
    /// как справочник и/или
    /// грузить данные из managed файла.
    ///
    /// Выполняет enrichment модели:
    /// - дополняет отсутствующие данные;
    /// - вычисляет что-нибудь;
    /// - подготавливает модель
    ///   к трансформации;
    /// - сверяется со справочниками
    ///   или сторонними сервисами.
    ///
    /// В MVP:
    /// - имитирует получение кода из ОКЕИ.
    ///
    /// В проде возможно:
    /// - запросы к сторонним API;
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
    public class OkeiEnricher : IDataEnricher
    {
        private readonly ILogger<OkeiEnricher> _logger;

        private static readonly Dictionary<string, string> _okeiMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["ШТ"] = "796",
                ["КГ"] = "166",
                ["Л"] = "112"
            };

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="logger"></param>
        public OkeiEnricher(ILogger<OkeiEnricher> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Логика наполнения модели данными, которых нет в инпуте
        /// </summary>
        /// <param name="model">Модель для наполнения</param>
        /// <returns>Возвращает воркера в тредпул, чтобы поделал что-то полезное, пока ждём IO, которого нет в текущей реализации</returns>
        public Task EnrichAsync(ImportDeclaration model)
        {
            _logger.LogInformation("OkeiEnricher Model enrichment started");

            int counterSuccess = 0;
            int counterFailure = 0;

            foreach (var product in model.Products)
            {
                if (string.IsNullOrWhiteSpace(product.UnitCode))
                {
                    _logger.LogWarning(
                        "Product unit code is empty. Product name: {ProductName}.",
                        product.Name);

                    counterFailure++;

                    continue;
                }

                var normalized = product.UnitCode.Trim();

                if (_okeiMap.TryGetValue(normalized, out var okeiCode))
                {
                    product.UnitCode = okeiCode;

                    counterSuccess++;
                }
                else
                {
                    _logger.LogWarning(
                        "Unknown OKEI unit: {Unit}. Product: {ProductName}.",
                        normalized, product.Name);

                    counterFailure++;

                    continue;
                }
            }

            _logger.LogInformation(
                "OkeiEnricher finished. Success: {counterSuccess}. Failed: {counterFailure}.",
                counterSuccess,
                counterFailure);

            return Task.CompletedTask;
        }
    }
}
