using DemoETL.Extraction;


/// <summary>
/// Извлекает значение под якорем.
///
/// Пример:
///
///     ИНН
///     1234567890
///
/// Используется
/// для вертикальных макетов.
/// </summary>
public class BelowLabelStrategy : IExtractionStrategy
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
