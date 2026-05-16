using DemoETL.Domain.Models;
using DemoETL.Extraction;
using DemoETL.Extraction.Configuration;
using DemoETL.Extraction.Extractors;
using Microsoft.Extensions.Logging;


namespace DemoETL.Mapping;

/*
 * Generic "Рабочая лошадка" движка; вызывается парсером, получает от него extraction конфиг с описанием полей в гриде экселя, из которых экстракторы достают данные,
 * дёргает нужные экстракторы с их логикой извлечения данных,
 * мапает (перекладывает) данные в нашу модель.
 * 
 * Логируем шаги для потенциального тюнинга конфигов извлечения и/или добавления новых экстракторов.
 * 
 * TODO: переделать после имплементации IFieldExtractionStrategy, смотри паттерн document processing systems - separating traversal from interpretation
 */

/// <summary>
/// Маппинг извлечённых данных
/// в нашу нормализованную модель.
///
/// Маппер:
/// - преобразует извлечённые данные;
/// - строит нашу каноничную модель;
/// - объединяет извлечённые фрагменты данных.
///
/// Важно:
/// маппер:
/// - не должен читать Excel/макет;
/// - не должен выполнять генерацию XML;
/// - не должен знать XSLT/XSD.
///
/// Маппер работает:
///     примитивы и сырые данные → наша модель.
///
/// Является границей между:
/// - слоем извлечения;
/// - слоем предметной области domain.
/// </summary>
public class Mapper
{
    private readonly SimpleExtractor _extractor;
    private readonly TableExtractor _tableExtractor;
    private readonly ILogger<Mapper> _logger;

    /// <summary>
    /// Собираем экстракторы/стратегии/навигаторы
    /// </summary>
    /// <param name="extractor"></param>
    /// <param name="tableExtractor"></param>
    /// <param name="logger"></param>
    public Mapper(
        SimpleExtractor extractor,
        TableExtractor tableExtractor,
        ILogger<Mapper> logger)
    {
        _extractor = extractor;
        _tableExtractor = tableExtractor;
        _logger = logger;
    }

    /// <summary>
    /// Читаем и перекладываем данные в нашу модель.
    /// </summary>
    /// <param name="sheets">Листы эксель</param>
    /// <param name="config">Конфигурация извлечения</param>
    /// <returns>Нормализованная модель</returns>
    /// <exception cref="Exception">Падаем, если не получили листов эксель</exception>
    public ImportDeclaration Map(
        List<SheetGrid> sheets,
        ExtractionConfig config)
    {
        _logger.LogInformation("Mapping started");

        if (sheets == null || sheets.Count == 0)
            throw new Exception("No sheets provided");

        var model = new ImportDeclaration() { Contract = new() };

        foreach (var sheet in sheets)
        {
            _logger.LogInformation("Processing sheet: {Sheet}", sheet.Name);

            // Шапка дока
            //
            // ИНН

            var innRaw = _extractor.FindFirstNonEmptyRightOfLabels(
                sheet,
                config.Fields.Inn);

            if (!string.IsNullOrWhiteSpace(innRaw))
            {
                model.Inn = innRaw;
            
                _logger.LogInformation("INN found: {Inn}", model.Inn);
            }
            
            // КПП (в текущих эксель его всё равно нет, используем enricher-заглушку)
            var kppRaw = _extractor.FindFirstNonEmptyRightOfLabels(
                sheet,
                config.Fields.Kpp);

            if (!string.IsNullOrWhiteSpace (kppRaw))
            {
                model.Kpp = kppRaw;

                _logger.LogInformation("KPP found: {Kpp}", model.Kpp);
            }

            // Наименование организации (покупателя ака налогоплательщика)
            var organizationNameRaw = _extractor.FindFirstNonEmptyRightOfLabelInRowContaining(
                sheet,
                config.Fields.OrganizationContextLabels,
                config.Fields.OrganizationTargetLabel[0]);

            if (!string.IsNullOrWhiteSpace(organizationNameRaw))
            {
                model.OrganizationName = organizationNameRaw;

                _logger.LogInformation(
                    "Organization found: {Organization}",
                    model.OrganizationName);
            }

            // Подписант
            var signerRaw = _extractor.FindValueBelowLabel(
                sheet,
                config.Fields.SignerName);

            if (!string.IsNullOrWhiteSpace(signerRaw))
            {
                model.Signer = ParseSigner(signerRaw);

                _logger.LogInformation(
                    "Signer extracted: {Signer}",
                    signerRaw);
            }

            // Признак договора лизинга
            var isLeasingRaw = _extractor.FindFirstNonEmptyRightOfLabels(
                sheet,
                config.Fields.LeasingMark);

            if (isLeasingRaw != null)
            {
                model.IsLeasing = (bool)ParseCheckbox(isLeasingRaw);
            }

            _logger.LogInformation(
                "Leasing flag: {Value}",
                model.IsLeasing);

            // Признак договора переработки давальческого сырья
            var isTollingRaw = _extractor.FindFirstNonEmptyRightOfLabels(
                sheet,
                config.Fields.TollingMark);

            if (isTollingRaw != null)
            {
                model.IsTolling = (bool)ParseCheckbox(isTollingRaw);
            }

            _logger.LogInformation(
                "Tolling flag: {Value}",
                model.IsTolling);

            // ПрПодп и ПВДок - значения по умолчанию (за неимением лучшего)
            model.Signer ??= new Signer();

            model.Signer.SignerType = Int32.Parse(
                config.Defaults.SignerType ?? "2");
            model.StatementReason = Int32.Parse(
                config.Defaults.StatementReason ?? "1");

            // БазаНДС
            var vatBaseTotalRaw = _extractor.FindValueByHierarchicalHeaders(
                sheet, config.Totals.RowAnchor,
                config.Totals.VatBaseTotalParentHeader, config.Totals.VatBaseTotalChildHeader);

            // TODO избавиться и перейти на ExtractionResult
            if (!string.IsNullOrWhiteSpace(vatBaseTotalRaw))
            {
                model.VatBaseTotal = ParseDecimal(vatBaseTotalRaw);
            }

            // ИтогоНДС
            var vatTotalRaw = _extractor.FindValueByHierarchicalHeaders(
                sheet, config.Totals.RowAnchor,
                config.Totals.VatTotalParentHeader, config.Totals.VatTotalChildHeader);

            if (!string.IsNullOrWhiteSpace(vatTotalRaw))
            {
                model.VatTotal = ParseDecimal(vatTotalRaw);
            }

            // ИтогоАкциз
            var exciseTotalRaw = _extractor.FindValueByHierarchicalHeaders(
                sheet, config.Totals.RowAnchor,
                config.Totals.ExciseTotalParentHeader, config.Totals.ExciseTotalChildHeader);

            if (!string.IsNullOrWhiteSpace(exciseTotalRaw))
            {
                model.ExciseTotal = ParseDecimal(exciseTotalRaw);
            }

            // Договор - СвКонтракт1
            //
            // ИдНомПродР1
            if (string.IsNullOrWhiteSpace(model.Contract.SellerId))
            {
                model.Contract.SellerId =
                     _extractor.FindFirstNonEmptyRightOfLabels(sheet, config.Contract.SellerId);

                if (!string.IsNullOrWhiteSpace(model.Contract.SellerId))
                {
                    _logger.LogInformation(
                        "SellerId found: {SellerId}",
                        model.Contract.SellerId);
                }
            }

            // ПрПродФЛ
            var sellerIsIndividualRaw =
                _extractor.FindFirstNonEmptyRightOfLabels(
                    sheet,
                    config.Contract.SellerIsIndividual);

            if (sellerIsIndividualRaw != null)
            {
                model.Contract.SellerIsIndividual = (bool)ParseCheckbox(sellerIsIndividualRaw);
            }

            _logger.LogInformation(
                "SellerIsIndividual flag: {Value}",
                model.Contract.SellerIsIndividual);

            // НаимПродР1
            if (string.IsNullOrWhiteSpace(model.Contract.SellerName))
            {
                model.Contract.SellerName =
                    _extractor.FindFirstNonEmptyRightOfLabelInRowContaining(
                        sheet,
                        new List<string>
                        {
                            "01",
                            "2"
                        },
                        "01");

                if (!string.IsNullOrWhiteSpace(model.Contract.SellerName))
                {
                    _logger.LogInformation(
                        "SellerName found: {SellerName}",
                        model.Contract.SellerName);
                }
            }

            // КодСтранПродР1
            if (string.IsNullOrWhiteSpace(model.Contract.SellerCountryCode))
            {
                model.Contract.SellerCountryCode =
                    _extractor.FindFirstNonEmptyRightOfLabels(sheet, config.Contract.SellerCountryCode);

                if (!string.IsNullOrWhiteSpace(model.Contract.SellerCountryCode))
                {
                    _logger.LogInformation(
                        "SellerCountryCode found: {SellerCountryCode}",
                        model.Contract.SellerCountryCode);
                }
            }

            // АдресПродР1
            if (string.IsNullOrWhiteSpace(model.Contract.SellerAddress))
            {
                model.Contract.SellerAddress =
                    _extractor.FindNthNonEmptyRightOfLabel(sheet, config.Contract.SellerAddress[0], 2);

                if (!string.IsNullOrWhiteSpace(model.Contract.SellerAddress))
                {
                    _logger.LogInformation(
                        "SellerAddress found: {SellerAddress}",
                        model.Contract.SellerAddress);
                }
            }

            // ИдНомПокР1
            if (string.IsNullOrWhiteSpace(model.Contract.BuyerId))
            {
                model.Contract.BuyerId =
                    _extractor.FindFirstNonEmptyRightOfLabels(sheet, config.Contract.BuyerId);

                if (!string.IsNullOrWhiteSpace(model.Contract.BuyerId))
                {
                    _logger.LogInformation(
                        "BuyerId found: {BuyerId}",
                        model.Contract.BuyerId);
                }
            }

            // НаимПокР1
            if (string.IsNullOrWhiteSpace(model.Contract.BuyerName))
            {
                model.Contract.BuyerName =
                    _extractor.FindFirstNonEmptyRightOfLabelInRowContaining(
                        sheet,
                        new List<string>
                        {
                            "01",
                            "2"
                        },
                        "2");

                if (!string.IsNullOrWhiteSpace(model.Contract.BuyerName))
                {
                    _logger.LogInformation(
                        "BuyerName found: {BuyerName}",
                        model.Contract.BuyerName);
                }
            }

            // КодСтранПокР1
            if (string.IsNullOrWhiteSpace(model.Contract.BuyerCountryCode))
            {
                model.Contract.BuyerCountryCode =
                    _extractor.FindNthNonEmptyRightOfLabel(sheet, config.Contract.BuyerCountryCode[0], 4);

                if (!string.IsNullOrWhiteSpace(model.Contract.BuyerCountryCode))
                {
                    _logger.LogInformation(
                        "BuyerCountryCode found: {BuyerCountryCode}",
                        model.Contract.BuyerCountryCode);
                }
            }

            // АдресПокР1
            if (string.IsNullOrWhiteSpace(model.Contract.BuyerAddress))
            {
                model.Contract.BuyerAddress =
                    _extractor.FindNthNonEmptyRightOfLabel(sheet, config.Contract.BuyerAddress[0], 5);

                if (!string.IsNullOrWhiteSpace(model.Contract.BuyerAddress))
                {
                    _logger.LogInformation(
                        "BuyerAddress found: {BuyerAddress}",
                        model.Contract.BuyerAddress);
                }
            }

            // СвКонтр1
            var contractAndSpecificationTargetRow =
                _extractor.FindRowByAnchor(
                    sheet,
                    config.Contract.ContractDocumentNumber[0]);

            if (contractAndSpecificationTargetRow != null)
            {
                // НомКонтр
                if (string.IsNullOrWhiteSpace(
                    model.Contract.ContractDocument?.Number))
                {
                    model.Contract.ContractDocument = new ContractDocument();
                    
                    model.Contract.ContractDocument?.Number =
                        _extractor.FindFirstNonEmptyRightOfLabel(
                            contractAndSpecificationTargetRow,
                            config.Contract.ContractDocumentNumber[0]);

                    if (!string.IsNullOrWhiteSpace(model.Contract.ContractDocument?.Number))
                    {
                        _logger.LogInformation(
                            "ContractDocument.Number found: {ContractDocument.Number}",
                            model.Contract.ContractDocument.Number);
                    }
                }

                // ДатаКонтр
                if (model.Contract.ContractDocument?.Date == null)
                {
                    var rawContractDocumentDate =
                        _extractor.FindFirstNonEmptyRightOfLabel(
                            contractAndSpecificationTargetRow,
                            config.Contract.ContractDocumentDate[0]);

                    if (DateTime.TryParseExact(
                            rawContractDocumentDate,
                            "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out var parsedContractDocumentDate))
                    {
                        model.Contract.ContractDocument.Date = parsedContractDocumentDate;

                        _logger.LogInformation(
                            "ContractDocument.Date found: {ContractDocument.Date}",
                            model.Contract.ContractDocument.Date);
                    }
                }

                // СвСпециф
                if (model.Contract.ContractDocument.Specification == null)
                {
                    // Наличие СвСпециф и оно же НомСпециф при наличии
                    // detect presence -> instantiate -> populate
                    var specificationPresenceAndNumber =
                        _extractor.FindFirstNonEmptyRightOfLabel(
                            contractAndSpecificationTargetRow,
                            config.Contract.SpecificationNumber[0]);

                    if (!string.IsNullOrWhiteSpace(specificationPresenceAndNumber))
                    {
                        _logger.LogInformation("Specification detected");    

                        model.Contract.ContractDocument.Specification = new SpecificationInfo();

                        // НомСпециф
                        model.Contract.ContractDocument.Specification.SpecificationNumber
                            = specificationPresenceAndNumber;

                        _logger.LogInformation(
                            "SpecificationNumber found: {SpecificationNumber}",
                            model.Contract.ContractDocument.Specification.SpecificationNumber);

                        // ДатаСпециф
                        var rawSpecificationDate =
                            _extractor.FindFirstNonEmptyRightOfLabel(
                                contractAndSpecificationTargetRow,
                                config.Contract.SpecificationDate[0]);

                        if (DateTime.TryParseExact(
                            rawSpecificationDate,
                            "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out var parsedSpecificationDate))
                        {
                            model.Contract.ContractDocument.Specification.SpecificationDate =
                                parsedSpecificationDate;

                            _logger.LogInformation(
                                "SpecificationDate found: {SpecificationDate}",
                                 model.Contract.ContractDocument.Specification.SpecificationDate);
                        }

                        // НомПСпециф
                        // Заглушка. Работаем с одним листом, ContractDocument не предполагает коллекцию SpecificationInfo(СвСпециф) в данный момент
                        model.Contract?.ContractDocument?.Specification?.ApplicationNumber = "1";
                    }           
                }
            }
        }

        // ТЧ (Табличная часть)
        foreach (var sheet in sheets)
        {
            _logger.LogInformation(
                "Trying table extraction from sheet: {Sheet}",
                sheet.Name);

            var products = _tableExtractor.ExtractProducts(
                sheet,
                config.Table);

            if (products.Any())
            {
                _logger.LogInformation(
                    "Products extracted from sheet: {Sheet}, Count: {Count}",
                    sheet.Name,
                    products.Count);

                model.Products.AddRange(products);
            }
        }

        // Для дебага; при срабатывании следует проверить экстракшн конфиг и/или задуматься над добавлением новых экстракторов (логикой)
        if (string.IsNullOrWhiteSpace(model.Inn))
        {
            _logger.LogWarning("INN not found");
        }

        if (!model.Products.Any())
        {
            _logger.LogWarning("No products extracted");
        }

        _logger.LogInformation("Mapping completed");

        return model;
    }


    /// SignerType в реальности:
    /// либо используется enrichment;
    /// либо клиентские правила;
    /// либо ручной mapping;
    /// либо defaults/configuration.
    /// 
    /// В целом никакой проблемы добавить в экстракшн конфиг дока:
    /// "Defaults": {
    ///   "SignerType": 2
    /// }
    private Signer ParseSigner(string fio)
    {
        var parts = fio
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return new Signer
        {
            LastName = parts.Length > 0 ? parts[0] : "",
            FirstName = parts.Length > 1 ? parts[1] : "",
            MiddleName = parts.Length > 2 ? parts[2] : null,

            // TODO Пока stub.
            // SignerType = 2
        };
    }

    // TODO переделать в CheckboxExtractionResult для разделения: не найден якорь, пустой чекбокс.
    /// <summary>
    /// Парсинг отметок типа договоров
    /// </summary>
    /// <param name="value">Отметка "Х" в чекбоксе</param>
    /// <returns>Наличие отметки в чекбоксе или не найден якорь</returns>
    private bool? ParseCheckbox(string? value)
    {
        if (value == null)
            return null;

        return value.Trim()
            .Equals("Х", StringComparison.OrdinalIgnoreCase);
    }

    private decimal ParseDecimal(string value)
    {
        if (decimal.TryParse(value?.Replace(",", "."),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;

        return 0;
    }
}
