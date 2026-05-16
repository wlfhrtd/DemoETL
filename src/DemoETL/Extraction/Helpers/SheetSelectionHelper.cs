using DemoETL.Extraction;

/// <summary>
/// Выбираем листы эксель, которые будем обрабатывать.
/// 
/// Sheets в конфигах извлечения.
/// Примеры: 1, 3, 4-7
/// </summary>
public static class SheetSelectionHelper
{
    /// <summary>
    /// У парсера есть конфиг извлечения
    /// получаем значение Sheets.
    /// Парсим, фильтруем.
    /// </summary>
    /// <param name="sheets">Весь набор листов эксель после чтения ExcelReader</param>
    /// <param name="expression">Значение Sheets из конфига извлечения</param>
    /// <returns>Листы для обработки</returns>
    public static List<SheetGrid> Filter(
        List<SheetGrid> sheets,
        string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return sheets;

        var indexes = Parse(expression);

        return sheets
            .Where((s, i) => indexes.Contains(i + 1))
            .ToList();
    }

    private static HashSet<int> Parse(string expr)
    {
        var result = new HashSet<int>();

        foreach (var part in expr.Split(','))
        {
            if (part.Contains('-'))
            {
                var range = part.Split('-');

                var start = int.Parse(range[0]);
                var end = int.Parse(range[1]);

                for (int i = start; i <= end; i++)
                    result.Add(i);
            }
            else
            {
                result.Add(int.Parse(part));
            }
        }

        return result;
    }
}
