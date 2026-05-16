using DemoETL.Domain.Models;
using DemoETL.Extraction.Configuration;
using DemoETL.Extraction.Interfaces;
using DemoETL.Mapping;


namespace DemoETL.Extraction.Parsers
{
    /// <summary>
    /// Парсер документа:
    ///     Заявление о ввозе товаров
    ///     и уплате косвенных налогов.
    ///     КНД 1110017
    ///     Приказ ФНС РФ от 13.12.2019 N ММВ-7-6/634@
    ///     ZVLRPOK
    ///     
    /// Оркестрация для конкретного типа документа.
    /// 
    /// Сейчас нарушены границы:
    ///     - Mapper - гибрид парсера и маппера;
    ///     - Mapper знает про Extraction, чего быть не должно.
    /// В будущем:
    ///     - нужно отделить стратегии извлечения;
    ///     - парсер знает стратегии извлечения;
    ///     - маппер не знает стратегии извлечения и макет;
    ///     - парсер выбирает стратегию;
    ///     - маппер получает raw результат извлечения.
    ///
    /// Использует:
    /// - конфигурацию извлечения;
    /// - (должен использовать) экстракторы/стратегии;
    /// - маппер.
    ///
    /// Знает:
    /// - (должен знать) особенности макета;
    /// - особенности документа;
    /// - порядок извлечения;
    /// - сугубо оркестрация, менеджмент и provisioning.
    ///
    /// Может содержать:
    /// - особенности макетов;
    /// - предположения по извлечению;
    /// - выбор стратегий извлечения.
    ///
    /// Не должен:
    /// - генерировать XML;
    /// - выполнять XSLT;
    /// - валидировать XML.
    /// </summary>
    public class ZVLRPOKParser : IDocumentParser
    {
        /// <summary>
        /// Тип документа.
        /// Пока строки,
        /// потом, возможно, enum.
        /// </summary>
        public string Type => "ZVLRPOK";

        private readonly Mapper _mapper;
        private readonly ExtractionConfigProvider _configProvider;

        /// <summary>
        /// Конструктор с маппером и сервисом конфигов извлечения
        /// </summary>
        /// <param name="mapper">Экземпляр маппера</param>
        /// <param name="configProvider">Сервис конфигов извлечения</param>
        public ZVLRPOKParser(Mapper mapper, ExtractionConfigProvider configProvider)
        {
            _mapper = mapper;
            _configProvider = configProvider;
        }

        /// <summary>
        /// Оркестрация процесса обработки эксель
        /// </summary>
        /// <param name="sheets">Список листов эксель</param>
        /// <returns>Наша нормализованная модель</returns>
        public ImportDeclaration Parse(List<SheetGrid> sheets)
        {
            var config = _configProvider.Load("ZVLRPOK");

            /*
             * выбираем листы для обработки
             * 
             * Почему именно в парсере:
             * какая-то логика в парсере может решить,
             * что значение Sheets из конфига
             * не является конечным.
             */
            var selectedSheets =
                SheetSelectionHelper.Filter(
                    sheets,
                    config.Sheets);

            return _mapper.Map(selectedSheets, config);
        }
    }
}
