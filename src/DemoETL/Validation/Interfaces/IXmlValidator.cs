namespace DemoETL.Validation.Interfaces
{
    /// <summary>
    /// Абстракция XML валидатора.
    ///
    /// Валидатор:
    /// - проверяет итоговый XML;
    /// - гарантирует соответствие XSD-схеме.
    ///
    /// Возможные реализации:
    /// - XSD валидация;
    /// - бизнес валидация;
    /// - валидация внешними сервисами.
    ///
    /// Важно:
    /// валидатор работает:
    ///     после трансформации.
    /// </summary>
    public interface IXmlValidator
    {   
        /// <summary>
        /// Валидируем конечный XML на соответствие XSD-схеме
        /// </summary>
        /// <param name="xmlPath">Путь к файлу конечного XML</param>
        /// <param name="xsdPath">Пусть к файлу XSD-схемы</param>
        void Validate(string xmlPath, string xsdPath);
    }
}
