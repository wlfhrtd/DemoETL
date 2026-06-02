using DemoETL.Domain.Models;
using System.Xml.Linq;


namespace DemoETL.Transformation.Builders
{
    /*
     * Строит нормализованный/промежуточный xml по нашей модели.
     * 
     * Пример xml на выходе:
     * <Root>
      <Sender>
        <INN>7802579868</INN>
        <KPP>780201001</KPP>
        <Name>ООО Ромашка</Name>
      </Sender>
      <Products>
        <Product>
          <Name>Товар 1</Name>
          <Quantity>100</Quantity>
          <Price>500000</Price>
        </Product>
      </Products>
    </Root>
     */

    /// <summary>
    /// Билдер промежуточного XML.
    ///
    /// Преобразует:
    ///     нашу модель
    ///
    /// в:
    ///     наш нормализованный
    ///     промежуточный XML.
    ///
    /// Промежуточный XML намеренно:
    /// - не равен XML ФНС;
    /// - является репрезентацией
    ///   для внутреннего использования.
    ///
    /// Используется:
    /// - перед XSLT трансформацией.
    ///
    /// Позволяет:
    /// - отделить доменную модель от целевой XML-схемы.
    /// </summary>
    public class XmlBuilder
    {
        /// <summary>
        /// Строим промежуточный XML.
        /// </summary>
        /// <param name="model">Наша нормализованная модель</param>
        /// <returns>Промежуточный XML</returns>
        public XDocument Build(ImportDeclaration model, DocumentContext documentContext)
        {
            return new XDocument(
                new XElement("Root",
                    new XElement("IdFileSuffix", documentContext.IdFileSuffix),
                    // СвОтпр
                    new XElement("Sender",
                        new XElement("INN", model.Inn ?? ""),
                        new XElement("KPP", model.Kpp ?? ""),
                        new XElement("Name", model.OrganizationName ?? ""),
                        new XElement("TaxAuthority", model.TaxAuthority ?? "")
                    ),
                    // Подписант
                    new XElement("Signer",
                        new XElement("Type", model.Signer?.SignerType),
                        new XElement("LastName", model.Signer?.LastName),
                        new XElement("FirstName", model.Signer?.FirstName),
                        new XElement("MiddleName", model.Signer?.MiddleName)
                    ),
                    // СвЗвл
                    new XElement("Declaration",
                        new XElement("LeasingFlag", model.IsLeasing ? "1" : "0"),
                        new XElement("TollingFlag", model.IsTolling ? "1" : "0"),
                        new XElement("StatementReason", model.StatementReason),
                        new XElement("VatBaseTotal", model.VatBaseTotal),
                        new XElement("ExciseTotal", model.ExciseTotal),
                        new XElement("VatTotal", model.VatTotal),

                        // СвКонтракт1
                        new XElement("ContractInfo",
                            new XElement("SellerId", model.Contract?.SellerId),
                            new XElement("SellerIsIndividual", model.Contract.SellerIsIndividual ? "1" : "0"),
                            new XElement("SellerName", model.Contract?.SellerName),
                            new XElement("SellerCountryCode", model.Contract?.SellerCountryCode),
                            new XElement("SellerAddress", model.Contract?.SellerAddress),
                            new XElement("BuyerId", model.Contract?.BuyerId),
                            new XElement("BuyerName", model.Contract?.BuyerName),
                            new XElement("BuyerCountryCode", model.Contract?.BuyerCountryCode),
                            new XElement("BuyerAddress", model.Contract?.BuyerAddress),
                            // СвКонтр1
                            new XElement("ContractDocument",
                                new XElement("ContractDocumentNumber", model.Contract?.ContractDocument?.Number),
                                new XElement("ContractDocumentDate", model.Contract?.ContractDocument?.Date?.ToString("dd.MM.yyyy")),
                                // СвСпециф
                                model.Contract?.ContractDocument?.Specification != null
                                ?
                                new XElement("SpecificationInfo",
                                    new XElement("SpecificationApplicationNumber", model.Contract.ContractDocument.Specification.ApplicationNumber),
                                    new XElement("SpecificationNumber", model.Contract.ContractDocument.Specification.SpecificationNumber),
                                    new XElement("SpecificationDate", model.Contract.ContractDocument.Specification.SpecificationDate?.ToString("dd.MM.yyyy"))
                                )
                                : null
                            )
                        )
                    ),
                        new XElement("Products",
                            model.Products.Select(p =>
                                new XElement("Product",
                                    new XElement("Number", p.Number),
                                    new XElement("Name", p.Name ?? ""),
                                    new XElement("TnVedCode", p.TnVedCode ?? ""),
                                    new XElement("UnitCode", p.UnitCode ?? ""),
                                    new XElement("Quantity", p.Quantity),
                                    new XElement("Price", p.Price),
                                    new XElement("CurrencyCode", p.CurrencyCode ?? ""),
                                    new XElement("CurrencyRate", p.CurrencyRate),
                                    new XElement("CurrencyMultiplier", p.CurrencyMultiplier),
                                    new XElement("InvoiceNumber", p.InvoiceNumber ?? ""),
                                    new XElement("InvoiceDate", p.InvoiceDate?.ToString("dd.MM.yyyy")),
                                    new XElement("AcceptanceDate", p.AcceptanceDate?.ToString("dd.MM.yyyy")),
                                    new XElement("ExciseBase", p.ExciseBase),
                                    new XElement("ExciseUnitCode", p.ExciseUnitCode ?? ""),
                                    new XElement("VatBase", p.VatBase),
                                    new XElement("ExciseRateFixed", p.ExciseRateFixed),
                                    new XElement("ExciseRateAdValorem", p.ExciseRateAdValorem),
                                    new XElement("VatRate", p.VatRate),
                                    new XElement("ExciseAmount", p.ExciseAmount),
                                    new XElement("VatAmount", p.VatAmount),
                                    new XElement("IsExciseExempt", p.IsExciseExempt ? "0" : "1"),
                                    new XElement("IsVatExempt", p.IsVatExempt ? "0" : "1"),
                                    // СвТСД
                                    new XElement("TransportDocumentsInfo",
                                        p.TransportDocuments.Select(tsd =>
                                            new XElement("TransportDocument",
                                                new XElement("Number", tsd.Number ?? ""),
                                                new XElement("Date", tsd.Date.ToString("dd.MM.yyyy"))
                                            )
                                        )
                                    )
                            )
                        )
                    )
                )
            
            );
        }
    }
}
