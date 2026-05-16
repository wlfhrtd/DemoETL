namespace DemoETL.Extraction.Configuration;

/// <summary>
/// Конфигурация извлечения.
/// Модели описания полей в гриде экселя,
/// из которых экстракторы достают данные
/// во время обработки маппером.
/// Смотри Extraction/Configs.
///
/// Описывает:
/// - метки/ярлыки;
/// - якоря;
/// - правила извлечения.
///
/// Загружается из JSON.
///
/// Config-driven извлечение позволяет:
/// - адаптироваться к разным макетам;
/// - не перекомпилировать проект;
/// - поддерживать разные XLS генераторы.
///
/// Должно использоваться в:
/// - парсерах;
/// - экстракторах;
/// - стратегиях извлечения.
/// </summary>
public class ExtractionConfig
{
    /// <summary>
    /// Поля для обработки
    /// </summary>
    public ImportDeclarationConfig Fields { get; set; } = new();

    /// <summary>
    /// Табличная часть
    /// </summary>
    public TableConfig Table { get; set; } = new();

    /// <summary>
    /// Листы для обработки
    /// 
    /// Пример:
    /// "Sheets": "1,3-5"
    /// </summary>
    public string? Sheets { get; set; }

    /// <summary>
    /// Значения по умолчанию для некоторых полей
    /// </summary>
    public DefaultsConfig Defaults { get; set; } = new();

    /// <summary>
    /// Totals: ндс, суммы и прочее
    /// </summary>
    public TotalsConfig Totals { get; set; } = new();

    /// <summary>
    /// Информация о договоре, спецификации
    /// СвКонтракт1
    /// </summary>
    public ContractConfig Contract { get; set; } = new();
}
