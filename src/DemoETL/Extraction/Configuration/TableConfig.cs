namespace DemoETL.Extraction.Configuration;

/// <summary>
/// Табличная часть
/// </summary>
public class TableConfig
{   
    /// <summary>
    /// Хедер табличной части
    /// </summary>
    public List<string> Header { get; set; } = new();

    /// <summary>
    /// Если в наименованном заголовке таблицы
    /// присутствуют дополнительные строки,
    /// например, | 1 | 2 | 3 | 4 | 5 | ...
    /// или пустые строки,
    /// то можем "перепрыгнуть", применив оффсет
    /// </summary>
    public int VerticalOffsetToFirstProduct { get; set; }

    // Колонки табличной части

    /// <summary>
    /// НаимТов
    /// </summary>
    public List<string> Name { get; set; } = new();

    /// <summary>
    /// ТНВЭД
    /// </summary>
    public List<string> TnVedCode { get; set; } = new();

    /// <summary>
    /// ЕдИзмТов
    /// </summary>
    public List<string> UnitCode { get; set; } = new();

    /// <summary>
    /// КоличТов
    /// </summary>
    public List<string> Quantity { get; set; } = new();

    /// <summary>
    /// СтоимТов
    /// </summary>
    public List<string> Price { get; set; } = new();

    /// <summary>
    /// ВалТов
    /// </summary>
    public List<string> CurrencyCode { get; set; } = new();

    /// <summary>
    /// КурсВал
    /// </summary>
    public List<string> CurrencyRate { get; set; } = new();

    /// <summary>
    /// БазаВал
    /// 
    /// Количество единиц валюты за 1 (один) российский рубль. Значение по умолчанию 1.
    /// При ВалТов=643 значение элемента равно 1
    /// </summary>
    public List<string> CurrencyMultiplier { get; set; } = new();

    /// <summary>
    /// СерНомТСД
    /// </summary>
    public List<string> TransportDocumentNumber { get; set; } = new();

    /// <summary>
    /// ДатаТСД
    /// </summary>
    public List<string> TransportDocumentDate { get; set; } = new();

    /// <summary>
    /// НомСчФ
    /// </summary>
    public List<string> InvoiceNumber { get; set; } = new();

    /// <summary>
    /// ДатаСчФ
    /// </summary>
    public List<string> InvoiceDate { get; set; } = new();

    /// <summary>
    /// ДатаПрин
    /// </summary>
    public List<string> AcceptanceDate { get; set; } = new();

    /// <summary>
    /// НБАкциз
    /// </summary>
    public List<string> ExciseBase { get; set; } = new();

    /// <summary>
    /// ЕдИзмТовНБАкц
    /// </summary>
    public List<string> ExciseUnitCode { get; set; } = new();

    /// <summary>
    /// НБНДС
    /// </summary>
    public List<string> VatBase { get; set; } = new();

    /// <summary>
    /// СтАкцизТверд
    /// </summary>
    public List<string> ExciseRateFixed { get; set; } = new();

    /// <summary>
    /// СтАкцизАдвал
    /// </summary>
    public List<string> ExciseRateAdValorem { get; set; } = new();

    /// <summary>
    /// СтНДС
    /// </summary>
    public List<string> VatRate { get; set; } = new();

    /// <summary>
    /// СумАкциз
    /// </summary>
    public List<string> ExciseAmount { get; set; } = new();

    /// <summary>
    /// СумНДС
    /// </summary>
    public List<string> VatAmount { get; set; } = new();
}
