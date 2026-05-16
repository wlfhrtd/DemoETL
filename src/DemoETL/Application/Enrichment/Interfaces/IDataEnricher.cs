using DemoETL.Domain.Models;


namespace DemoETL.Application.Enrichment.Interfaces
{
    /// <summary>
    /// Абстракция enrichment слоя.
    ///
    /// Enrichment:
    /// - дополняет модель;
    /// - обогащает извлечённые данные;
    /// - выполняет внешние lookup операции.
    ///
    /// Возможные enrichment сценари для примера:
    /// - получение КПП по ИНН;
    /// - lookup ОКЕИ;
    /// - lookup ОКСМ;
    /// - интеграция с API.
    ///
    /// Важно:
    /// enricher:
    /// - не должен читать Excel;
    /// - не должен выполнять XSLT;
    /// - не должен валидировать XML.
    ///
    /// Работает только с domain моделью.
    /// </summary>
    public interface IDataEnricher
    {
        /// <summary>
        /// Основной метод с логикой наполнения модели
        /// </summary>
        /// <param name="model">Наполняемая модель</param>
        /// <returns></returns>
        Task EnrichAsync(ImportDeclaration model);
    }
}
