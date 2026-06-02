namespace DemoETL.Domain.Models
{
    /// <summary>
    /// Контекст документа.
    /// </summary>
    public sealed class DocumentContext
    {
        /// <summary>
        /// Для даты в ИдФайл
        /// Пример: ИдФайл="{concat('ON_ZVLRPOK_', Sender/TaxAuthority, '_', Sender/TaxAuthority, '_', Sender/INN, Sender/KPP, '_', $dateId, '_', IdFileSuffix)}"
        /// </summary>
        public string DateId { get; init; } = string.Empty;

        /// <summary>
        /// ДатаДок
        /// </summary>
        public string DateDoc { get; init; } = string.Empty;

        /// <summary>
        /// GUID. XSLT 1.0 не умеет в GUID.
        /// </summary>
        public string IdFileSuffix { get; init; } = string.Empty;
    }
}
