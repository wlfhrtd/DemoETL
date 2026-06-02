using DemoETL.Domain.Models;

namespace DemoETL.Transformation.Interfaces;

/// <summary>
/// Применяет xslt к xml.
///
/// Ответственность:
/// трансформация промежуточного нормализованного xml
/// в xml целевого формата,
/// например, xml по ФНС.
///
/// Абстракция конкретного xslt движка:
/// - XslCompiledTransform (.NET)
/// - SaxonCS (платный)
/// - кастомы
///
/// Позволит менять движок без изменения пайплайна/оркестрации.
///
/// Инпут:
///     нормализованный/промежуточный xml
///
/// На выходе:
///     трансформированный под целевой формат xml
///
/// Трансформер:
/// - не должен валидировать;
/// - не должен содержать бизнес логику;
/// - только применяет xslt правила;
/// - xsd-валидация в другом слое.
/// </summary>
public interface IXsltTransformer
{
    /// <summary>
    /// Применяет xslt.
    /// </summary>
    /// <param name="inputXmlPath">
    /// Путь к исходному xml.
    /// </param>
    /// <param name="xsltPath">
    /// Путь к xslt.
    /// </param>
    /// <param name="outputXmlPath">
    /// Путь к xml на выходе.
    /// </param>
    /// <param name="documentContext">
    /// Контекст документа.
    /// </param>
    void Transform(
        string inputXmlPath,
        string xsltPath,
        string outputXmlPath,
        DocumentContext documentContext);
}
