using DemoETL.Application.Configuration;
using DemoETL.Application.Detection.Interfaces;
using DemoETL.Extraction;


namespace DemoETL.Application.Detection
{
    /// <summary>
    /// Один из определителей типа документов - config-based/config-driven.
    ///
    /// Определяет тип документа по описанию в Application/Configuration/documents.json:
    /// - по ключевым словам;
    /// - по содержимому Excel;
    /// - без жёсткого хардкода.
    ///
    /// Detection rules:
    /// - задаются в documents.json;
    /// - могут изменяться без перекомпиляции.
    ///
    /// Используется pipeline перед выбором парсера.
    ///
    /// Поддерживает:
    /// - разные типы документов;
    /// - нестрогое определение (score-based);
    /// - нулевое доверие к инпуту.
    ///
    /// Важно:
    /// detector:
    /// - не парсит документ;
    /// - не маппит данные;
    /// - только определяет тип.
    /// </summary>
    public class ConfigDocumentTypeDetector : IDocumentTypeDetector
    {
        private readonly AppConfig _config;

        /// <summary>
        /// Конструктор с получением конфига метаданных доков (documents.json)
        /// </summary>
        /// <param name="config">Метаданные документов/конфиг</param>
        public ConfigDocumentTypeDetector(AppConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Score-based определение: тип документа с наибольшим кол-вом хитов по DetectionKeywords из конфига выигрывает.
        /// </summary>
        /// <param name="sheets">Список листов экселя</param>
        /// <returns>Сейчас возвращает строку, в будущем следует перейти на перечисление (смотри Domain/Enums/DocumentType). Или нет - новые доки потребуют рекомпиляции.</returns>
        /// <exception cref="Exception">Падаем, если не смогли определить тип документа</exception>
        public string Detect(List<SheetGrid> sheets)
        {
            var scores = new Dictionary<string, int>();

            foreach (var doc in _config.Documents)
            {
                scores[doc.Key] = 0;

                foreach (var keyword in doc.Value.DetectionKeywords)
                {
                    foreach (var sheet in sheets)
                    {
                        foreach (var row in sheet.Rows)
                        {
                            foreach (var cell in row.Cells)
                            {
                                var value = cell.Value?.ToLower();

                                if (value != null && value.Contains(keyword.ToLower()))
                                {
                                    scores[doc.Key]++;
                                }
                            }
                        }
                    }
                }
            }

            var best = scores
                .OrderByDescending(x => x.Value)
                .First();

            if (best.Value == 0)
                throw new Exception("Document type not detected. Check DetectionKeywords in config");

            return best.Key;
        }
    }
}
