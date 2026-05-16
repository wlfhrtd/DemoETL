using DemoETL.Extraction;


namespace DemoETL.Application.Detection.Interfaces
{
    /// <summary>
    /// Абстракция механизма определения типа документа.
    /// Парсер резолвится из DI (смотри Infrastructure/ServiceCollectionExtensions) по documentType в пайплайне ещё до Extraction.
    /// 
    /// Позволяет:
    /// - менять стратегии определения типа документа;
    /// - не связывать pipeline с конкретной реализацией.
    ///
    /// Возможные реализации:
    /// - по ключевым словам;
    /// - по регулярным выражениям;
    /// - применять ML.
    ///
    /// На вход:
    ///     raw excel листы.
    ///
    /// На выход:
    ///     тип документа.
    /// </summary>
    public interface IDocumentTypeDetector
    {
        /// <summary>
        /// Логика определения типа документа
        /// </summary>
        /// <param name="sheets">Листы экселя</param>
        /// <returns></returns>
        string Detect(List<SheetGrid> sheets);
    }
}
