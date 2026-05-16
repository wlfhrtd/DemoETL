namespace DemoETL.Extraction.Configuration
{
    /// <summary>
    /// Модель якорей для поиска полей
    /// </summary>
    public class ImportDeclarationConfig
    {
        /// <summary>
        /// Якорь для ИНН
        /// </summary>
        public List<string> Inn { get; set; } = new();

        /// <summary>
        /// Якорь для КПП
        /// </summary>
        public List<string> Kpp { get; set; } = new();

        /// <summary>
        /// Список якорей для поиска строки
        /// с наименованием организации
        /// </summary>
        public List<string> OrganizationContextLabels { get; set; } = new();

        /// <summary>
        /// Якорь, рядом с которым наименование организации
        /// </summary>
        public List<string> OrganizationTargetLabel { get; set; } = new();

        /// <summary>
        /// Якорь для поиска ФИО подписанта
        /// </summary>
        public List<string> SignerName { get; set; } = new();

        /// <summary>
        /// Якорь для поиска отметки/чекбокса
        /// признака договора лизинга
        /// </summary>
        public List<string> LeasingMark { get; set; } = new();

        /// <summary>
        /// Якорь для поиска отметки/чекбокса
        /// признака договора переработки
        /// давальческого сырья
        /// </summary>
        public List<string> TollingMark { get; set; } = new();

    }
}
