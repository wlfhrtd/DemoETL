namespace DemoETL.Extraction.Configuration;

/// <summary>
/// Договор
/// </summary>
public class ContractConfig
{
    /// <summary>
    /// ИдНомПродР1
    /// </summary>
    public List<string> SellerId { get; set; } = new();

    /// <summary>
    /// ПрПродФЛ
    /// </summary>
    public List<string> SellerIsIndividual { get; set; } = new();

    /// <summary>
    /// НаимПродР1
    /// </summary>
    public List<string> SellerName { get; set; } = new();

    /// <summary>
    /// КодСтранПродР1
    /// </summary>
    public List<string> SellerCountryCode { get; set; } = new();

    /// <summary>
    /// АдресПродР1
    /// </summary>
    public List<string> SellerAddress { get; set; } = new();

    /// <summary>
    /// ИдНомПокР1
    /// </summary>
    public List<string> BuyerId { get; set; } = new();

    /// <summary>
    /// НаимПокР1
    /// </summary>
    public List<string> BuyerName { get; set; } = new();

    /// <summary>
    /// КодСтранПокР1
    /// </summary>
    public List<string> BuyerCountryCode { get; set; } = new();

    /// <summary>
    /// АдресПокР1
    /// </summary>
    public List<string> BuyerAddress { get; set; } = new();

    /// <summary>
    /// НомКонтр
    /// </summary>
    public List<string> ContractDocumentNumber { get; set; } = new();

    /// <summary>
    /// ДатаКонтр
    /// </summary>
    public List<string> ContractDocumentDate { get; set; } = new();

    /// <summary>
    /// НомПСпециф
    /// 
    /// Нумерация по порядку, начиная с номера 1
    /// = количество SpecificationInfo (СвСпециф) в документе.
    /// </summary>
    public List<string> SpecificationApplicationNumber { get; set; } = new();

    /// <summary>
    /// НомСпециф
    /// </summary>
    public List<string> SpecificationNumber { get; set; } = new();

    /// <summary>
    /// ДатаСпециф
    /// </summary>
    public List<string> SpecificationDate { get; set; } = new();
}
