using DemoETL.Validation.Interfaces;
using System.Xml;
using System.Xml.Schema;


namespace DemoETL.Validation.Validators
{
    /*
     * Простой валидатор xml по xsd.
     * Валидирует конечный xml по ФНС уже после применения xslt к нашему нормализованному/промежуточному xml из нашей модели.
     * 
     * Нюанс: если валидируем по своим xsd, то по xsd конечного потребителя xml можем и не проскочить.
     * Вывод: стремимся получить и использовать максимально точные и полные xsd,
     * а также делать и проверять логи и артефакты.
     */

    /// <summary>
    /// XSD валидатор.
    ///
    /// Проверяет:
    /// - структуру XML;
    /// - обязательные элементы;
    /// - типы данных;
    /// - констрейнты схемы.
    ///
    /// Используется:
    /// - после XSLT трансформации.
    ///
    /// В проде возможно:
    /// - цепочки валидаторов;
    /// - валидация по бизнес логике;
    /// - валидация внешними сервисами.
    /// </summary>
    public class XsdValidator : IXmlValidator
    {
        /// <summary>
        /// Валидируем конечный XML на соответствие XSD-схеме
        /// </summary>
        /// <param name="xmlPath">Путь к файлу конечного XML</param>
        /// <param name="xsdPath">Пусть к файлу XSD-схемы</param>
        /// <exception cref="Exception">Бросаем ошибку XSD</exception>
        public void Validate(string xmlPath, string xsdPath)
        {
            var schemas = new XmlSchemaSet();
            schemas.Add("", xsdPath);

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemas
            };

            settings.ValidationEventHandler += (s, e) =>
            {
                throw new Exception($"XSD Error: {e.Message}");
            };

            using var reader = XmlReader.Create(xmlPath, settings);

            while (reader.Read()) { }
        }
    }
}
