using DemoETL.Domain.Models;
using DemoETL.Extraction.Configuration;
using System.Data;
using System.Text.RegularExpressions;


namespace DemoETL.Extraction.Extractors
{
    /// <summary>
    /// Экстрактор табличных данных.
    ///
    /// Выполняет:
    /// - поиск заголовка таблицы;
    /// - определение колонок;
    /// - извлечение строк таблицы.
    ///
    /// Поддерживает:
    /// - config-driven обработку таблиц.
    /// </summary>
    public class TableExtractor
    {
        /// <summary>
        /// Извлекаем строки табличной части
        /// </summary>
        /// <param name="sheet">Лист эксель</param>
        /// <param name="config">Конфиг для таблицы</param>
        /// <returns>Список Product'ов - строк ТЧ</returns>
        public List<Product> ExtractProducts(
            SheetGrid sheet,
            TableConfig config)
        {
            var products = new List<Product>();

            // ищем header
            var headerRow =
                FindHeaderContainingLabels(sheet, config.Header);

            if (headerRow == null)
                return products;

            // вычисляем индексы колонок
            var columnIndexes =
                ResolveColumnIndexes(sheet, headerRow, config);

            // старт данных
            var startIndex =
                headerRow.Index + config.VerticalOffsetToFirstProduct;

            foreach (var row in sheet.Rows.Where(r => r.Index >= startIndex))
            {
                var name =
                    GetCell(row, columnIndexes.Name);

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                if (name.Contains(
                        "ИТОГО",
                        StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                var product = new Product
                {
                    Name = name,

                    TnVedCode =
                        GetCell(row, columnIndexes.TnVedCode),

                    UnitCode =
                        GetCell(row, columnIndexes.UnitCode),

                    Quantity =
                        ParseDecimal(
                            GetCell(row, columnIndexes.Quantity)),

                    Price =
                        ParseDecimal(
                            GetCell(row, columnIndexes.Price)),

                    CurrencyCode =
                        GetCell(row, columnIndexes.CurrencyCode),

                    CurrencyRate =
                        ParseDecimal(
                            GetCell(row, columnIndexes.CurrencyRate)),

                    CurrencyMultiplier =
                        ParseDecimal(config.CurrencyMultiplier[0]),

                    InvoiceNumber =
                        GetCell(row, columnIndexes.InvoiceNumber),

                    InvoiceDate =
                        ParseDate(GetCell(row, columnIndexes.InvoiceDate)),

                    AcceptanceDate =
                        ParseDate(GetCell(row, columnIndexes.AcceptanceDate)),

                    ExciseBase =
                        ParseDecimal(
                            GetCell(row, columnIndexes.ExciseBase)),

                    ExciseUnitCode = GetCell(row, columnIndexes.ExciseUnitCode),

                    VatBase =
                        ParseDecimal(
                            GetCell(row, columnIndexes.VatBase)),

                    ExciseRateFixed =
                        ParseDecimal(
                            GetCell(row, columnIndexes.ExciseRateFixed)),

                    ExciseRateAdValorem =
                        ParseDecimal(
                            GetCell(row, columnIndexes.ExciseRateAdValorem)),

                    VatRate = 
                        ParseDecimal(
                            GetCell(row, columnIndexes.VatRate)),

                    ExciseAmount =
                        ParseDecimal(
                            GetCell(row, columnIndexes.ExciseAmount)),

                    VatAmount =
                        ParseDecimal(
                            GetCell(row, columnIndexes.VatAmount)),

                    TransportDocuments = new List<TransportDocumentsInfo>
                    {
                        new TransportDocumentsInfo
                        {
                            Date = ParseDate(GetCell(row, columnIndexes.TransportDocumentDate)),

                            Number = GetCell(row, columnIndexes.TransportDocumentNumber)
                        }
                    }
                };

                products.Add(product);
            }

            return products;
        }

        /// <summary>
        /// Получаем индексы колонок
        /// из конфига
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="headerRow">Строка заголовка</param>
        /// <param name="config">Конфиг с именами колонок</param>
        /// <returns>Структура с индексами</returns>
        private ProductTableColumnIndexes ResolveColumnIndexes(SheetGrid sheet, Row headerRow, TableConfig config)
        {
            var result = new ProductTableColumnIndexes();

            result.Name = GetColumn(headerRow, config.Name[0]);
            result.TnVedCode = GetColumn(headerRow, config.TnVedCode[0]);
            result.UnitCode = GetColumn(headerRow, config.UnitCode[0]);
            result.Quantity = GetColumn(headerRow, config.Quantity[0]);
            result.Price = GetColumn(headerRow, config.Price[0]);

            result.CurrencyCode = FindColumnByHierarchicalHeaders(
                sheet, headerRow, config.CurrencyCode[0], config.CurrencyCode[1]);
            result.CurrencyRate = FindColumnByHierarchicalHeaders(
                sheet, headerRow, config.CurrencyRate[0], config.CurrencyRate[1]);

            result.InvoiceNumber = FindColumnByHeaderPath(
                sheet, headerRow, config.InvoiceNumber);
            result.InvoiceDate = FindColumnByHeaderPath(
                sheet, headerRow, config.InvoiceDate);

            result.AcceptanceDate = GetColumn(headerRow, config.AcceptanceDate[0]);

            var exciseColumns = FindColumnsUnderSpanningSubHeader(
                sheet, headerRow, config.ExciseBase[0], config.ExciseBase[1]);
            result.ExciseBase = exciseColumns.First(); // занимает 3 ячейки, поэтому юнит код только в 4й
            result.ExciseUnitCode = GetMiddleColumn(exciseColumns);

            result.VatBase = FindColumnByHierarchicalHeaders(
                sheet, headerRow, config.VatBase[0], config.VatBase[1]);

            result.ExciseRateFixed = FindColumnByHeaderPath(
                sheet, headerRow, config.ExciseRateFixed);
            result.ExciseRateAdValorem = FindColumnByHeaderPath(
                sheet, headerRow, config.ExciseRateAdValorem);

            result.VatRate = FindColumnByHierarchicalHeaders(
                sheet, headerRow, config.VatRate[0], config.VatRate[1]);

            result.ExciseAmount = FindColumnByHierarchicalHeaders(
                sheet, headerRow, config.ExciseAmount[0], config.ExciseAmount[1]);
            result.VatAmount = FindColumnByHierarchicalHeaders(
                sheet, headerRow, config.VatAmount[0], config.VatAmount[1]);

            result.TransportDocumentNumber = FindColumnByHeaderPath(
                sheet, headerRow, config.TransportDocumentNumber);
            result.TransportDocumentDate = FindColumnByHeaderPath(
                sheet, headerRow, config.TransportDocumentDate);

            return result;
        }

        /// <summary>
        /// Устойчивый парсер дат
        /// в формате день.Месяц.год
        /// </summary>
        /// <param name="date">Строковая дата</param>
        /// <returns>Дату типа DateTime</returns>
        /// <exception cref="ArgumentException">Падаем, если строка пустая или null</exception>
        /// <exception cref="FormatException">Падаем, если не подошло под формат</exception>
        private DateTime ParseDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                throw new ArgumentException(
                    "Date string cannot be null or empty.",
                    nameof(date));

            var normalized = NormalizeDate(date);

            var formats = new[]
            {
                "dd.MM.yyyy",
                "d.MM.yyyy",
                "dd.M.yyyy",
                "d.M.yyyy"
            };

            if (DateTime.TryParseExact(
                    normalized,
                    formats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var parsedDate))
            {
                return parsedDate;
            }

            throw new FormatException(
                $"Unable to parse date '{date}' using format dd.MM.yyyy.");
        }

        /// <summary>
        /// Санитайзер для строк
        /// с датами
        /// </summary>
        /// <param name="input">Строка с датой</param>
        /// <returns>Нормализованную строку с датой</returns>
        private string NormalizeDate(string input)
        {
            if (input == null)
                return string.Empty;

            var normalized = input
                .Replace('\u00A0', ' ')
                .Replace('\u2007', ' ')
                .Replace('\u202F', ' ')
                .Trim()
                .Replace("года", "")
                .Replace("г.", "")
                .Replace("г", "")
                .Trim();

            normalized = Regex.Replace(normalized, @"\s+", " ");

            return normalized;
        }

        /// <summary>
        /// Получаем индекс средней
        /// колонки из списка колонок
        /// </summary>
        /// <param name="columns">Список колонок</param>
        /// <returns>Индекс средней колонки</returns>
        private int GetMiddleColumn(List<int> columns)
        {
            return columns[columns.Count / 2];
        }

        /// <summary>
        /// Эвристический метод поиска колонки.
        /// Например, для поиска ЕдИзмТовНБАкц,
        /// предполагая, что значения в ячейке
        /// этой колонки НЕ парсятся в decimal
        /// и длиной менее 10 знаков.
        /// Можно доработать заменив 10
        /// на переменную из аргументов метода.
        /// </summary>
        /// <param name="row">Строка, в которой ищем</param>
        /// <param name="candidateColumns">Список индексов колонок-кандидатов</param>
        /// <returns>Индекс подходящей колонки</returns>
        private int FindUnitCodeColumn(
            Row row,
            List<int> candidateColumns)
        {
            foreach (var col in candidateColumns)
            {
                var value = GetCell(row, col);

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                // ДМ3 / 796 / 112 и тд
                if (value.Length <= 10
                    && !decimal.TryParse(value, out _))
                {
                    return col;
                }
            }

            return -1;
        }

        /// <summary>
        /// Эвристический поиск колонки,
        /// содержащей decimal.
        /// </summary>
        /// <param name="row">Строка поиска</param>
        /// <param name="candidateColumns">Индексы колонок для проверки</param>
        /// <returns></returns>
        private int FindDecimalColumn(
            Row row,
            List<int> candidateColumns)
        {
            foreach (var col in candidateColumns)
            {
                var value = GetCell(row, col);

                if (decimal.TryParse(value, out _))
                {
                    return col;
                }
            }

            return -1;
        }

        /// <summary>
        /// Ищем индекс колонки
        /// для ячейки, находящейся
        /// по пути заголовков и
        /// подзаголовков.
        /// Например,
        /// "InvoiceNumber": [ "Счет-фактура", "", "номер" ]
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="startRow">Самая верхняя строка</param>
        /// <param name="path">Якори на пути</param>
        /// <returns>Индекс ячейки</returns>
        private int FindColumnByHeaderPath(
            SheetGrid sheet,
            Row startRow,
            List<string> path)
        {
            int currentColumn = -1;
            int currentRowIndex = startRow.Index;

            foreach (var headerPart in path)
            {
                var row = sheet.Rows
                    .FirstOrDefault(r => r.Index == currentRowIndex);

                if (row == null)
                    return -1;

                var candidates = row.Cells.AsEnumerable();

                // ограничиваем область поиска
                if (currentColumn != -1)
                {
                    candidates = candidates
                        .Where(c => c.ColumnIndex >= currentColumn);
                }

                var found = candidates
                    .FirstOrDefault(c =>
                        c.Value.Contains(
                            headerPart,
                            StringComparison.OrdinalIgnoreCase));

                if (found == null)
                    return -1;

                currentColumn = found.ColumnIndex;

                currentRowIndex++;
            }

            return currentColumn;
        }

        /// <summary>
        /// Получаем индекс колонки,
        /// содержащей строку.
        /// Например, индекс колонки
        /// "Наименование товара"
        /// в строке хедере.
        /// </summary>
        /// <param name="header">Строка грида</param>
        /// <param name="name">Токен для поиска</param>
        /// <returns>Индекс колонки</returns>
        private int GetColumn(Row header, string name)
        {
            var normalizedName =
                NormalizeText(name);

            var cell = header.Cells
                .FirstOrDefault(c =>
                    NormalizeText(c.Value)
                        .Contains(
                            normalizedName,
                            StringComparison.OrdinalIgnoreCase));

            return cell?.ColumnIndex ?? -1;
        }

        /// <summary>
        /// Получаем значение ячейки
        /// в строке по индексу колонки
        /// </summary>
        /// <param name="row">Строка поиска</param>
        /// <param name="col">Индекс ячейки</param>
        /// <returns>Значение в ячейке</returns>
        private string? GetCell(Row row, int col)
        {
            return row.Cells.FirstOrDefault(c => c.ColumnIndex == col)?.Value;
        }

        /// <summary>
        /// Парсер/нормализатор
        /// для чисел
        /// </summary>
        /// <param name="value">Строковое число</param>
        /// <returns>decimal вместо string</returns>
        private decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value?.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;

            return 0;
        }

        /// <summary>
        /// Ищем строку, содержащую
        /// список лейблов.
        /// Например, хедер ТЧ
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="requiredLabels">Метки в строке</param>
        /// <returns>Строка грида</returns>
        private Row? FindHeaderContainingLabels(
            SheetGrid sheet,
            List<string> requiredLabels)
        {
            foreach (var row in sheet.Rows)
            {
                // Проверяем, что строка содержит все требуемые якоря
                var containsAll = requiredLabels.All(required =>
                    row.Cells.Any(c =>
                        c.Value.Contains(
                            required,
                            StringComparison.OrdinalIgnoreCase)));

                if (!containsAll)
                    continue;

                return row;
            }

            return null;
        }

        /// <summary>
        /// Поиск ячейки под составным
        /// заголовком parent->child
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="headerRow">Строка с хедерами</param>
        /// <param name="parentHeader">Основной заголовок</param>
        /// <param name="childHeader">Подзаголовок</param>
        /// <returns>Индекс искомой ячейки</returns>
        private int FindColumnByHierarchicalHeaders(
            SheetGrid sheet,
            Row headerRow,
            string parentHeader,
            string childHeader)
        {
            // parent row
            var parentCell = headerRow.Cells
                .FirstOrDefault(c =>
                    c.Value.Contains(
                        parentHeader,
                        StringComparison.OrdinalIgnoreCase));

            if (parentCell == null)
                return -1;

            // child row под header
            var childRow = sheet.Rows
                .FirstOrDefault(r => r.Index == headerRow.Index + 1);

            if (childRow == null)
                return -1;

            // ищем child только справа от parent
            foreach (var cell in childRow.Cells
                         .Where(c => c.ColumnIndex >= parentCell.ColumnIndex)
                         .OrderBy(c => c.ColumnIndex))
            {
                if (cell.Value.Contains(
                        childHeader,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return cell.ColumnIndex;
                }
            }

            return -1;
        }

        /// <summary>
        /// Эвристический поиск
        /// группы колонок под
        /// охватывающим хедером.
        /// Умеет "перепрыгивать"
        /// пустые строки.
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="headerRow">Строка хедера</param>
        /// <param name="parentHeader">Заголовок</param>
        /// <param name="childHeader">Подзаголовок</param>
        /// <returns>Список индексов искомой группы ячеек</returns>
        private List<int> FindColumnsUnderSpanningSubHeader(
            SheetGrid sheet,
            Row headerRow,
            string parentHeader,
            string childHeader)
        {
            var result = new List<int>();

            var parentCell = headerRow.Cells
                .FirstOrDefault(c =>
                    c.Value.Contains(
                        parentHeader,
                        StringComparison.OrdinalIgnoreCase));

            if (parentCell == null)
                return result;

            var childRow = sheet.Rows
                .FirstOrDefault(r => r.Index == headerRow.Index + 1);

            if (childRow == null)
                return result;

            var childCell = childRow.Cells
                .FirstOrDefault(c =>
                    c.ColumnIndex >= parentCell.ColumnIndex &&
                    c.Value.Contains(
                        childHeader,
                        StringComparison.OrdinalIgnoreCase));

            if (childCell == null)
                return result;

            int startColumn = childCell.ColumnIndex;

            // следующая непустая ячейка = конец группы
            var nextHeader = childRow.Cells
                .Where(c =>
                    c.ColumnIndex > startColumn &&
                    !string.IsNullOrWhiteSpace(c.Value))
                .OrderBy(c => c.ColumnIndex)
                .FirstOrDefault();

            int endColumn =
                nextHeader != null
                    ? nextHeader.ColumnIndex - 1
                    : childRow.Cells.Max(c => c.ColumnIndex);

            for (int i = startColumn; i <= endColumn; i++)
            {
                result.Add(i);
            }

            return result;
        }

        /// <summary>
        /// Санитайзер для текста
        /// в ячейках.
        /// Против типичных невидимых
        /// символов в эксель.
        /// </summary>
        /// <param name="value">Грязный текст из эксель</param>
        /// <returns>Нормализованный текст</returns>
        private string NormalizeText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("\u00A0", " ");

            return Regex.Replace(value, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Служебная структура
        /// для удобного сбора
        /// индексов колонок ТЧ
        /// </summary>
        private struct ProductTableColumnIndexes
        {
            public int Name { get; set; }
            public int TnVedCode { get; set; }
            public int UnitCode { get; set; }
            public int Quantity { get; set; }
            public int Price { get; set; }
            public int CurrencyCode { get; set; }
            public int CurrencyRate { get; set; }
            public int TransportDocumentNumber { get; set; }
            public int TransportDocumentDate { get; set; }
            public int InvoiceNumber { get; set; }
            public int InvoiceDate { get; set; }
            public int AcceptanceDate { get; set; }
            public int ExciseBase { get; set; }
            public int ExciseUnitCode { get; set; }
            public int VatBase { get; set; }
            public int ExciseRateFixed { get; set; }
            public int ExciseRateAdValorem { get; set; }
            public int VatRate { get; set; }
            public int ExciseAmount { get; set; }
            public int VatAmount { get; set; }
        }
    }
}
