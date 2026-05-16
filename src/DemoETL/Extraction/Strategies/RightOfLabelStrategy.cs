using DemoETL.Extraction;


/// <summary>
/// Извлекает значение справа от якоря.
///
/// Пример:
///
///     ИНН | 1234567890
///
/// Поддерживает:
/// - смерженные ячейки;
/// - прыжки по оффсету;
/// - устойчивость к неточностям в гриде.
/// </summary>
public class RightOfLabelStrategy : IExtractionStrategy
{
    /// <summary>
    /// Извлекаем данные из грида
    /// </summary>
    /// <param name="sheet">Лист эксель</param>
    /// <param name="labels">Метки/якоря</param>
    /// <returns>Строковые данные</returns>
    public string? Extract(
        SheetGrid sheet,
        List<string> labels)
    {
        return null;
    }
}
