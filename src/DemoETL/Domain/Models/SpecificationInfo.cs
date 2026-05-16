namespace DemoETL.Domain.Models
{
    /// <summary>
    /// Спецификация - СвСпециф
    /// </summary>
    public class SpecificationInfo
    {
        /// <summary>
        /// НомПСпециф
        /// 
        /// Должна быть коллекция спецификаций
        /// НомПСпециф определяется по их количеству в коллекции
        /// TODO Пока заглушка в Mapper
        /// </summary>
        public string? ApplicationNumber { get; set; }

        /// <summary>
        /// НомСпециф
        /// </summary>
        public string? SpecificationNumber { get; set; }

        /// <summary>
        /// ДатаСпециф
        /// </summary>
        public DateTime? SpecificationDate { get; set; }
    }
}
