using System.Text.Json;


namespace DemoETL.Extraction.Configuration
{
    /// <summary>
    /// Провайдер сервис конфигов извлечения.
    ///
    /// Загружает:
    /// - JSON конфиги извлечения;
    /// - правила извлечения по типу дока.
    ///
    /// Используется парсерами.
    ///
    /// Позволяет:
    /// - хранить правила извлечения вне кода;
    /// - быстро адаптироваться к новым макетам.
    /// </summary>
    public class ExtractionConfigProvider
    {
        /// <summary>
        /// Основной метод загрузки конфига извлечения
        /// </summary>
        /// <param name="documentType">Тип документа</param>
        /// <returns>Конфиг извлечения данных</returns>
        /// <exception cref="Exception">Падаем, если битый конфиг</exception>
        public ExtractionConfig Load(string documentType)
        {
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Configs",
                $"{documentType}.json");

            var json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip, // разрешил комменты в json
                AllowTrailingCommas = true,                     // QoL: в конце последнего элемента можем ставить запятую, чтобы удобнее было добавлять новые элементы
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<ExtractionConfig>(
                       json,
                       options)
                   ?? throw new Exception(
                   "Failed to deserialize extraction config");
        }
    }
}
