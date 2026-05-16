namespace DemoETL.Domain.Models
{
    /// <summary>
    /// Договор, контракт, доп.соглашение и тд
    /// СвКонтр1
    /// </summary>
    public class ContractDocument
    {
        /// <summary>
        /// НомКонтр
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// ДатаКонтр
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Спецификация - СвСпециф
        /// 
        /// TODO Должна быть коллекция спецификаций
        /// НомПСпециф определяется по их количеству в коллекции
        /// </summary>
        public SpecificationInfo? Specification { get; set; }
    }
}
