namespace DemoETL.Extraction.Strategies
{
    // aka FindRightOfLabelInRowContaining aka contextual extraction
    /*
     * config
     * 
     * 
  "OrganizationName": {
    "Strategy": "RightOfLabelInContext",
    "ContextLabels": [ "01", "2" ],
    "TargetLabel": "2"
  }

    
    Суть: поиск по нескольим якорям, извлечение из найденной строки, извлечение относительно целевого якоря в строке.
     */

     /// <summary>
     /// NotImplementedYet
     /// </summary>
    public class RightOfLabelInContextStrategy
    {
        public string? Extract(
            SheetGrid sheet,
            List<string> contextLabels,
            string targetLabel)
        {
            foreach (var row in sheet.Rows)
            {
                var hasAllContext =
                    contextLabels.All(label =>
                        row.Cells.Any(c =>
                            c.Value.Contains(
                                label,
                                StringComparison.OrdinalIgnoreCase)));

                if (!hasAllContext)
                    continue;

                var targetCell = row.Cells.FirstOrDefault(c =>
                    c.Value.Equals(
                        targetLabel,
                        StringComparison.OrdinalIgnoreCase));

                if (targetCell == null)
                    continue;

                var rightCells = row.Cells
                    .Where(c => c.ColumnIndex > targetCell.ColumnIndex)
                    .OrderBy(c => c.ColumnIndex);

                var value = rightCells
                    .FirstOrDefault(c =>
                        !string.IsNullOrWhiteSpace(c.Value));

                if (value != null)
                    return value.Value;
            }

            return null;
        }
    }
}
