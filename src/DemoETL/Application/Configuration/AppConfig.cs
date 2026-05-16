using System.Text.Json;


namespace DemoETL.Application.Configuration
{
    /// <summary>
    /// Модель для определения типа документа
    /// и описаний обработки/метаданных доков
    /// в Application/Configuration/documents.json
    /// </summary>
    public class DocumentMetadata
    {
        /// <summary>
        /// Имя XSLT-файла,
        /// используемого для преобразования
        /// промежуточного XML
        /// в целевой формат.
        ///
        /// Пример:
        ///     ON_ZVLRPOK_5_08.xslt
        /// </summary>
        public string Xslt { get; set; } = string.Empty;

        /// <summary>
        /// Имя XSD-схемы,
        /// используемой для валидации
        /// итогового XML.
        ///
        /// Пример:
        ///     ON_ZVLRPOK_5_08.xsd
        /// </summary>
        public string Xsd { get; set; } = string.Empty;

        /// <summary>
        /// Набор ключевых слов,
        /// используемых для определения
        /// типа документа.
        ///
        /// Если detector находит
        /// данные слова в Excel,
        /// документ считается
        /// соответствующим типу.
        ///
        /// Определение намеренно нечёткое,
        /// так как входные XLS/XLSX:
        /// - с нулевым доверием;
        /// - могут отличаться макетом;
        /// - могут генерироваться разным ПО.
        ///
        /// Пример:
        ///     "заявление о ввозе товаров"
        ///     "косвенных налогов"
        /// </summary>
        public List<string> DetectionKeywords { get; set; } = new();
    }

    /// <summary>
    /// Корневая конфигурация приложения.
    ///
    /// Содержит:
    /// - список поддерживаемых типов документов;
    /// - метаданные документов;
    /// - пути к XSLT/XSD;
    /// - правила определения типа документа.
    ///
    /// Загружается из:
    /// - в publish билде    Configs/documents.json
    /// - в проекте          Application/Configuration/documents.json
    ///
    /// Используется:
    /// - Pipeline (оркестрация)
    /// - DocumentTypeDetector
    /// - Validator (в Pipeline)
    /// - XsltTransformer (в Pipeline)
    ///
    /// Позволяет:
    /// - добавлять новые типы документов без перекомпиляции;
    /// - делать detection config-driven;
    /// - отвязывать pipeline от конкретных документов.
    ///
    /// Пример:
    ///     ZVLRPOK
    ///     INVOICE
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Зарегистрированные типы документов.
        ///
        /// Ключ:
        ///     код/идентификатор документа.
        ///
        /// Значение:
        ///     метаданные документа.
        ///
        /// Пример:
        ///     "ZVLRPOK"
        /// </summary>
        public Dictionary<string, DocumentMetadata> Documents { get; set; } = new();

        /// <summary>
        /// Ридер json-конфига
        /// </summary>
        /// <param name="path">Путь до json-конфига</param>
        /// <returns>Модель с конфигом</returns>
        /// <exception cref="Exception">Выбрасываем при невалидном конфиге</exception>
        public static AppConfig Load(string path)
        {
            var json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip, // разрешил комменты в json
                AllowTrailingCommas = true,                     // QoL: в конце последнего элемента можем ставить запятую, чтобы удобнее было добавлять новые элементы
                PropertyNameCaseInsensitive = true
            };

            var config = JsonSerializer.Deserialize<AppConfig>(
                json,
                options);

            if (config == null)
                throw new Exception("Unable to load config");

            if (config.Documents == null || config.Documents.Count == 0)
                throw new Exception("Invalid config: Documents section is empty");

            return config;
        }
    }
}
