namespace DemoETL.Domain.Models
{
    /// <summary>
    /// СвКонтракт1
    /// </summary>
    public class ContractInfo
    {
        /// <summary>
        /// ИдНомПродР1
        /// </summary>
        public string? SellerId { get; set; }

        /// <summary>
        /// ПрПродФЛ
        /// </summary>
        public bool SellerIsIndividual { get; set; }

        /// <summary>
        /// НаимПродР1
        /// </summary>
        public string? SellerName { get; set; }

        /// <summary>
        /// КодСтранПродР1
        /// </summary>
        public string? SellerCountryCode { get; set; }

        /// <summary>
        /// АдресПродР1
        /// </summary>
        public string? SellerAddress { get; set; }

        /// <summary>
        /// ИдНомПокР1
        /// </summary>
        public string? BuyerId { get; set; }

        /// <summary>
        /// НаимПокР1
        /// </summary>
        public string? BuyerName { get; set; }

        /// <summary>
        /// КодСтранПокР1
        /// </summary>
        public string? BuyerCountryCode { get; set; }

        /// <summary>
        /// АдресПокР1
        /// </summary>
        public string? BuyerAddress { get; set; }

        /// <summary>
        /// СвКонтр1
        /// </summary>
        public ContractDocument? ContractDocument { get; set; }
    }
}
