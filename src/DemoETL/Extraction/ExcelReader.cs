using ClosedXML.Excel;


namespace DemoETL.Extraction
{
    /// <summary>
    /// Честный raw-reader.
    /// 
    /// Выполняет:
    /// - чтение Excel;
    /// - определение макета;
    /// - нормализацию ячеек;
    /// - xlsx -> in-memory grid.
    ///
    /// Важно:
    /// reader:
    /// - не знает бизнес логику;
    /// - не знает тип документа;
    /// - не знает XML.
    ///
    /// </summary>
    public class ExcelReader
    {   
        /// <summary>
        /// Основной метод чтения входного эксель
        /// </summary>
        /// <param name="path">Путь к файлу эксель</param>
        /// <returns>Список листов эксель</returns>
        public List<SheetGrid> Read(string path)
        {
            var result = new List<SheetGrid>();

            using var workbook = new XLWorkbook(path);

            foreach (var ws in workbook.Worksheets)
            {
                var sheet = new SheetGrid
                {
                    Name = ws.Name
                };

                var usedRange = ws.RangeUsed();

                foreach (var r in usedRange.RowsUsed())
                {
                    var row = new Row { Index = r.RowNumber() };

                    foreach (var c in r.Cells())
                    {
                        row.Cells.Add(new Cell
                        {
                            ColumnIndex = c.Address.ColumnNumber,
                            Value = Normalize(c.GetValue<string>())
                        });
                    }

                    sheet.Rows.Add(row);
                }

                result.Add(sheet);
            }

            return result;
        }

        private string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input
                .Trim()
                .Replace("\n", " ")
                .Replace("\r", " ");
        }
    }
}
