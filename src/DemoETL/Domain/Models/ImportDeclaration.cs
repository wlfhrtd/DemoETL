namespace DemoETL.Domain.Models
{
    /// <summary>
    /// Нормализованная доменная модель.
    /// Упрощена для демонстрации.
    /// 
    /// Представляет:
    ///     заявление о ввозе товаров
    ///     ZVLRPOK
    ///     КНД 1110017
    ///     Приказ ФНС РФ от 13.12.2019 N ММВ-7-6/634@
    ///
    /// Является:
    /// - внутренней моделью ETL;
    /// - промежуточным слоем абстракции;
    /// - независимой от Excel/XML.
    ///
    /// Важно:
    /// модель намеренно:
    /// - не зависит от макетов эксель;
    /// - не зависит от XML структуры ФНС;
    /// - не зависит от XSD.
    ///
    /// Это позволяет:
    /// - менять извлечение;
    /// - менять трансформацию;
    /// - переиспользовать модель.
    ///
    /// </summary>
    public class ImportDeclaration
    {
        // Шапка

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// КПП
        /// </summary>
        public string Kpp { get; set; } = string.Empty;

        /// <summary>
        /// Наименование организации
        /// </summary>
        public string OrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// КодНО
        /// </summary>
        public string TaxAuthority { get; set; } = string.Empty;

        /// <summary>
        /// Подписант
        /// </summary>
        public Signer? Signer { get; set; }

        /// <summary>
        /// Признак договора лизинга
        /// </summary>
        public bool IsLeasing { get; set; }

        /// <summary>
        /// Признак договора переработки давальческого сырья
        /// </summary>
        public bool IsTolling { get; set; }

        /// <summary>
        /// Причина возникновения заявления
        /// </summary>
        public int StatementReason { get; set; }

        /// <summary>
        /// Договор - СвКонтракт1
        /// </summary>
        public required ContractInfo Contract { get; set; }

        // Табличная часть

        /// <summary>
        /// Список строк в табличной части
        /// Например, товаров
        /// </summary>
        public List<Product> Products { get; set; } = new();

        // Подвал, тоталы

        /// <summary>
        /// БазаНДС
        /// </summary>
        public decimal VatBaseTotal { get; set; }

        /// <summary>
        /// ИтогоНДС
        /// </summary>
        public decimal VatTotal { get; set; }

        /// <summary>
        /// ИтогоАкциз
        /// </summary>
        public decimal ExciseTotal { get; set; }
    }
}
