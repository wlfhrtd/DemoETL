using DemoETL.Extraction;

/// <summary>
/// Абстракция стратегии извлечения.
///
/// Стратегия:
/// - знает как извлекать значение;
/// - знает детали макета;
/// - знает правила навигации по гриду.
///
/// Примеры:
/// - RightOfLabel;
/// - BelowLabel;
/// - Offset;
/// - Regex;
/// - MergedCells.
///
/// Используется парсерами.
///
/// Стратегия намеренно:
/// - не знает тип документа;
/// - не знает XML;
/// - не знает доменную модель.
/// </summary>
public interface IExtractionStrategy
{   
    /// <summary>
    /// Извлекаем данные из грида
    /// </summary>
    /// <param name="sheet">Лист эксель</param>
    /// <param name="labels">Метки/якоря</param>
    /// <returns>Строковые данные</returns>
    string? Extract(
        SheetGrid sheet,
        List<string> labels);
}
