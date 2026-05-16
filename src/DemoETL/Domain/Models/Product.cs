namespace DemoETL.Domain.Models
{
    /// <summary>
    /// СвТовар
    /// </summary>
    public class Product
    {
        /// <summary>
        /// НомТовПП
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// НаимТов
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// ТНВЭД
        /// </summary>
        public string? TnVedCode { get; set; }

        /// <summary>
        /// ЕдИзмТов
        /// </summary>
        public string? UnitCode { get; set; }

        /// <summary>
        /// КоличТов
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// СтоимТов
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// ВалТов
        /// </summary>
        public string? CurrencyCode { get; set; }

        /// <summary>
        /// КурсВал
        /// </summary>
        public decimal CurrencyRate { get; set; }

        /// <summary>
        /// БазаВал
        /// </summary>
        public decimal CurrencyMultiplier { get; set; }

        /// <summary>
        /// НомСчФ
        /// </summary>
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// ДатаСчФ
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// ДатаПрин
        /// </summary>
        public DateTime? AcceptanceDate { get; set; }

        /// <summary>
        /// НБАкциз
        /// </summary>
        public decimal ExciseBase { get; set; }

        /// <summary>
        /// ЕдИзмТовНБАкц
        /// </summary>
        public string? ExciseUnitCode { get; set; }

        /// <summary>
        /// НБНДС
        /// </summary>
        public decimal VatBase { get; set; }

        /// <summary>
        /// СтАкцизТверд
        /// </summary>
        public decimal? ExciseRateFixed { get; set; }

        /// <summary>
        /// СтАкцизАдвал
        /// </summary>
        public decimal? ExciseRateAdValorem { get; set; }

        /// <summary>
        /// СтНДС
        /// </summary>
        public decimal VatRate { get; set; }

        /// <summary>
        /// СумАкциз
        /// </summary>
        public decimal ExciseAmount { get; set; }

        /// <summary>
        /// СумНДС
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// ПрОсвАкциз
        /// </summary>
        public bool IsExciseExempt { get; set; }

        /// <summary>
        /// ПрОсвНДС
        /// </summary>
        public bool IsVatExempt { get; set; }

        /// <summary>
        /// СвТСД
        /// </summary>
        public required List<TransportDocumentsInfo> TransportDocuments { get; set; } = new();
    }

    /// <summary>
    /// СвТСД
    /// </summary>
    public class TransportDocumentsInfo
    {
        /// <summary>
        /// СерНомТСД
        /// </summary>
        public required string Number { get; set; }

        /// <summary>
        /// ДатаТСД
        /// </summary>
        public DateTime Date { get; set; }
    }
}
