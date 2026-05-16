namespace DemoETL.Extraction
{
    /// <summary>
    /// Нормализованное представление листа Excel.
    ///
    /// Представляет:
    /// - лист;
    /// - строки;
    /// - ячейки.
    ///
    /// Используется в слое извлечения.
    /// </summary>
    public class SheetGrid
    {   
        /// <summary>
        /// Имя листа Excel
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Список строк в листе
        /// </summary>
        public List<Row> Rows { get; set; } = new();
    }

    /// <summary>
    /// Модель строки в листе
    /// </summary>
    public class Row
    {   
        /// <summary>
        /// Индекс строки в листе
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Список ячеек в строке
        /// </summary>
        public List<Cell> Cells { get; set; } = new();
    }

    /// <summary>
    /// Модель ячейки в строке
    /// </summary>
    public class Cell
    {   
        /// <summary>
        /// Индекс ячейки в строке(колонка)
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Данные в ячейке
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}
