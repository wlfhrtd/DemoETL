namespace DemoETL.Extraction.Configuration;

/// <summary>
/// Модель для Totals: ндс, суммы и прочее
/// </summary>
public class TotalsConfig
{
    /// <summary>
    /// Якорь строки
    /// </summary>
    public string RowAnchor { get; set; } = string.Empty;

    /// <summary>
    /// Parent в смерженном хедере - "Налоговая база"
    /// для БазаНДС
    /// </summary>
    public string? VatBaseTotalParentHeader { get; set; }

    /// <summary>
    /// Child в смерженном хедере - "НДС"
    /// для БазаНДС
    /// </summary>
    public string? VatBaseTotalChildHeader { get; set; }

    /// <summary>
    /// Parent в смерженном хедере - "Суммы налогов"
    /// для ИтогоНДС
    /// </summary>
    public string? VatTotalParentHeader { get; set; }

    /// <summary>
    /// Child в смерженном хедере - "НДС"
    /// для ИтогоНДС
    /// </summary>
    public string? VatTotalChildHeader { get; set; }

    /// <summary>
    /// Parent для ИтогоАкциз
    /// </summary>
    public string? ExciseTotalParentHeader { get; set; }

    /// <summary>
    /// Child для ИтогоАкциз
    /// </summary>
    public string? ExciseTotalChildHeader { get; set; }
}
