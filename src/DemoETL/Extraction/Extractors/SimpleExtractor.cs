namespace DemoETL.Extraction.Extractors
{
    /// <summary>
    /// Простой экстрактор для демонстрации.
    /// Используем label-based подход,
    /// устойчивый к смещению данных в Excel.
    /// 
    /// Сейчас границы размыты для MVP,
    /// в будущем нужно выделить стратегии и примитивы.
    ///
    /// Выполняет:
    /// - поиск меток;
    /// - поиск значений;
    /// - простое извлечение из ячеек.
    ///
    /// Не должен содержать:
    /// - логику документа;
    /// - логику парсера;
    /// - логику маппера.
    ///
    /// Должен быть примитивен.
    /// </summary>
    public class SimpleExtractor
    {
        /// <summary>
        /// Cтратегия первая непустая ячейка справа; sparse-right scan
        /// </summary>
        /// <param name="sheet">Лист эксель</param>
        /// <param name="labels">Метки/якоря из конфига</param>
        /// <returns>Строку-значение с данными для нашей модели</returns>
        public string? FindFirstNonEmptyRightOfLabels(SheetGrid sheet, List<string> labels)
        {
            foreach (var row in sheet.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (labels.Any(l => cell.Value.Contains(l, StringComparison.OrdinalIgnoreCase)))
                    {
                        var right = row.Cells
                            .Where(c => c.ColumnIndex > cell.ColumnIndex)
                            .OrderBy(c => c.ColumnIndex)
                            .FirstOrDefault(c =>
                                !string.IsNullOrWhiteSpace(c.Value));

                        if (right != null && !string.IsNullOrWhiteSpace(right.Value))
                            return right.Value;
                    }
                }
            }

            return null;
        }

        // TODO: remove and use RightOfLabelInContextStrategy instead
        /// <summary>
        /// Ищем значение первой непустой ячейки
        /// справа от якорной метки
        /// в строке, строго содержащей
        /// список меток из конфига
        /// </summary>
        /// <param name="sheet">Лист эксель</param>
        /// <param name="requiredLabels">Метки, которые должна содержать строка</param>
        /// <param name="targetLabel">Якорь, справа от которого берём значение</param>
        /// <returns>Значение искомой ячейки</returns>
        public string? FindFirstNonEmptyRightOfLabelInRowContaining(
            SheetGrid sheet,
            List<string> requiredLabels,
            string targetLabel)
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

                // Ищем target якорь
                var targetCell = row.Cells.FirstOrDefault(c =>
                    c.Value.Equals(
                        targetLabel,
                        StringComparison.OrdinalIgnoreCase));

                if (targetCell == null)
                    continue;

                // Берём первое непустое справа
                var rightValue = row.Cells
                    .Where(c => c.ColumnIndex > targetCell.ColumnIndex)
                    .OrderBy(c => c.ColumnIndex)
                    .FirstOrDefault(c =>
                        !string.IsNullOrWhiteSpace(c.Value));

                if (rightValue != null)
                    return rightValue.Value;
            }

            return null;
        }

        // TODO: удалить и использовать соответствующую стратегию
        /// <summary>
        /// Ищем значение ячейки под якорной меткой.
        /// В данном варианте меток может быть несколько
        /// для улучшения эвристики.
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="labels">Метки, под которыми будем искать ячейку</param>
        /// <returns>Значение искомой ячейки</returns>
        public string? FindValueBelowLabel(
            SheetGrid sheet,
            List<string> labels)
        {
            for (int rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
            {
                var row = sheet.Rows[rowIndex];

                foreach (var cell in row.Cells)
                {
                    if (labels.Any(l =>
                            cell.Value.Contains(
                                l,
                                StringComparison.OrdinalIgnoreCase)))
                    {
                        if (rowIndex + 1 >= sheet.Rows.Count)
                            return null;

                        var nextRow = sheet.Rows[rowIndex + 1];

                        var firstNonEmpty = nextRow.Cells
                            .FirstOrDefault(c =>
                                !string.IsNullOrWhiteSpace(c.Value));

                        return firstNonEmpty?.Value;
                    }
                }
            }

            return null;
        }

        // стратегия - ячейка строго справа
        // TODO: переделать в стратегию
        /// <summary>
        /// Ищем значение первой ячейки
        /// строго справа от 
        /// первой совпавшей метки.
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="labels">Список меток для поиска</param>
        /// <returns></returns>
        public string? FindValueRightOfLabels(SheetGrid sheet, List<string> labels)
        {
            foreach (var row in sheet.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (labels.Any(l => cell.Value.Contains(l, StringComparison.OrdinalIgnoreCase)))
                    {
                        var right = row.Cells.FirstOrDefault(c => c.ColumnIndex == cell.ColumnIndex + 1);
                        if (right != null && !string.IsNullOrWhiteSpace(right.Value))
                            return right.Value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Для составных хедеров.
        /// Пример:
        ///
        /// |      НДС      |
        /// | база | сумма  |
        ///
        /// parentHeader = "НДС"
        /// childHeader  = "сумма"
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="parentHeader">Главный заголовок</param>
        /// <param name="childHeader">Подзаголовок</param>
        /// <returns>Индекс колонки, содержащей child header</returns>
        public int FindChildColumnUnderParent(
            SheetGrid sheet,
            string parentHeader,
            string childHeader)
        {
            foreach (var row in sheet.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (!cell.Value.Contains(
                            parentHeader,
                            StringComparison.OrdinalIgnoreCase))
                        continue;

                    var parentCol = cell.ColumnIndex;

                    // смотрим строку ниже
                    var nextRow = sheet.Rows
                        .FirstOrDefault(r => r.Index == row.Index + 1);

                    if (nextRow == null)
                        continue;

                    // ищем child справа в области parent
                    foreach (var childCell in nextRow.Cells)
                    {
                        if (childCell.ColumnIndex < parentCol)
                            continue;

                        if (childCell.Value.Contains(
                                childHeader,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return childCell.ColumnIndex;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Находим строку, содержащую метку
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="anchor">Метка-якорь</param>
        /// <returns>Строку грида</returns>
        public Row? FindRowByAnchor(
            SheetGrid sheet,
            string anchor)
        {
            return sheet.Rows.FirstOrDefault(r =>
                r.Cells.Any(c =>
                    c.Value.Contains(
                        anchor,
                        StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Ищем значение из ячейки
        /// в строке
        /// </summary>
        /// <param name="row">Строка грида</param>
        /// <param name="columnIndex">Индекс ячейки в строке</param>
        /// <returns>Значение ячейки</returns>
        public string? GetCellValue(
            Row row,
            int columnIndex)
        {
            return row.Cells
                .FirstOrDefault(c => c.ColumnIndex == columnIndex)
                ?.Value;
        }

        /// <summary>
        /// Ищет значение ячейки под составным заголовком
        /// и на пересечении с якорной строкой.
        /// Пример:
        ///
        ///         |      НДС      |
        ///         | база | сумма  |
        /// ИТОГО:
        /// 
        /// parentHeader = "НДС"
        /// childHeader  = "сумма"
        /// rowAnchor    = "ИТОГО"
        /// 
        /// Вернёт значение ячейки под "сумма"
        /// в строке ИТОГО
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="rowAnchor">Метка строки</param>
        /// <param name="parentHeader">Главный заголовок</param>
        /// <param name="childHeader">Подзаголовок</param>
        /// <returns>Значение искомой ячейки</returns>
        public string? FindValueByHierarchicalHeaders(
            SheetGrid sheet,
            string rowAnchor,
            string parentHeader,
            string childHeader)
        {
            var targetColumn =
                FindChildColumnUnderParent(
                    sheet,
                    parentHeader,
                    childHeader);

            if (targetColumn == -1)
                return null;

            var targetRow =
                FindRowByAnchor(sheet, rowAnchor);

            if (targetRow == null)
                return null;

            return GetCellValue(
                targetRow,
                targetColumn);
        }

        /// <summary>
        /// Ищет значение на пересечении:
        /// - строки, содержащей rowAnchor;
        /// - колонки, содержащей columnAnchor.
        ///
        /// Пример:
        /// rowAnchor    = "ИТОГО"
        /// columnAnchor = "Налоговая база"
        ///
        /// Вернёт значение ячейки на пересечении.
        ///
        /// Предполагается:
        /// - columnAnchor находится в header области;
        /// - rowAnchor находится в строке данных/итогов.
        /// </summary>
        public string? FindIntersectionValue(
            SheetGrid sheet,
            string rowAnchor,
            string columnAnchor)
        {
            // Ищем строку с rowAnchor
            var targetRow = sheet.Rows.FirstOrDefault(r =>
                r.Cells.Any(c =>
                    c.Value.Contains(
                        rowAnchor,
                        StringComparison.OrdinalIgnoreCase)));

            if (targetRow == null)
                return null;

            // Ищем колонку с columnAnchor
            int targetColumn = -1;

            foreach (var row in sheet.Rows)
            {
                var headerCell = row.Cells.FirstOrDefault(c =>
                    c.Value.Contains(
                        columnAnchor,
                        StringComparison.OrdinalIgnoreCase));

                if (headerCell != null)
                {
                    targetColumn = headerCell.ColumnIndex;
                    break;
                }
            }

            if (targetColumn == -1)
                return null;

            // Берём ячейку на пересечении
            var resultCell = targetRow.Cells
                .FirstOrDefault(c =>
                    c.ColumnIndex == targetColumn);

            if (resultCell == null)
                return null;

            return string.IsNullOrWhiteSpace(resultCell.Value)
                ? null
                : resultCell.Value;
        }

        /// <summary>
        /// Ищем значение N-нной непустой
        /// ячейки справа от метки
        /// </summary>
        /// <param name="sheet">Лист</param>
        /// <param name="label">Якорь</param>
        /// <param name="occurrence">Номер по счёту</param>
        /// <returns>Значение N-нной непустой</returns>
        public string? FindNthNonEmptyRightOfLabel(
            SheetGrid sheet,
            string label,
            int occurrence)
        {
            if (occurrence <= 0)
                return null;

            foreach (var row in sheet.Rows)
            {
                var cells = row.Cells
                    .OrderBy(c => c.ColumnIndex)
                    .ToList();

                for (int i = 0; i < cells.Count; i++)
                {
                    var cell = cells[i];

                    if (string.Equals(
                            cell.Value?.Trim(),
                            label.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        int foundCount = 0;

                        for (int j = i + 1; j < cells.Count; j++)
                        {
                            var value = cells[j].Value?.Trim();

                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                foundCount++;

                                if (foundCount == occurrence)
                                    return value;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Ищем значение первой
        /// непустой ячейки
        /// строго справа от метки
        /// </summary>
        /// <param name="row">Строка, в которой ищем</param>
        /// <param name="label">Якорь</param>
        /// <returns>Значение первой непустой ячейки</returns>
        public string? FindFirstNonEmptyRightOfLabel(
            Row row,
            string label)
        {
            var orderedCells = row.Cells
                .OrderBy(c => c.ColumnIndex)
                .ToList();

            for (int i = 0; i < orderedCells.Count; i++)
            {
                var cell = orderedCells[i];

                if (!cell.Value.Contains(
                        label,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                // ищем первую непустую справа
                for (int j = i + 1; j < orderedCells.Count; j++)
                {
                    var rightValue = orderedCells[j].Value?.Trim();

                    if (!string.IsNullOrWhiteSpace(rightValue))
                        return rightValue;
                }
            }

            return null;
        }


        // Пример использования
        //var contractDate = _extractor.FindFirstMatchingRightOfCell(
        //    row,
        //    "Дата договора",
        //    v => Regex.IsMatch(v, @"\d{2}\.\d{2}\.\d{4}"));
        /// <summary>
        /// Ищем значение ячейки,
        /// удовлетворяющее регулярке,
        /// находящееся в ячейке из строки,
        /// содержащей якорь
        /// </summary>
        /// <param name="row">Строка, содержащая метку</param>
        /// <param name="label">Метка</param>
        /// <param name="predicate">Предикат с регуляркой</param>
        /// <returns>Значение искомой ячейки</returns>
        public string? FindFirstMatchingRightOfCell(
            Row row,
            string label,
            Func<string, bool> predicate)
        {
            var orderedCells = row.Cells
                .OrderBy(c => c.ColumnIndex)
                .ToList();

            for (int i = 0; i < orderedCells.Count; i++)
            {
                var cell = orderedCells[i];

                if (!cell.Value.Contains(
                        label,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                for (int j = i + 1; j < orderedCells.Count; j++)
                {
                    var value = orderedCells[j].Value?.Trim();

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    if (predicate(value))
                        return value;
                }
            }

            return null;
        }
    }
}
