using DemoETL.Domain.Models;


namespace DemoETL.Application.Output
{
    /// <summary>
    /// Генератор имени файла
    /// </summary>
    public static class FnsFileNameGenerator
    {
        /// <summary>
        /// Генератор имени файла по приказу ФНС.
        /// Пока для одного формата.
        /// </summary>
        /// <param name="documentType">Тип документа</param>
        /// <param name="model">Каноничная модель</param>
        /// <param name="documentContext">Контекст документа</param>
        /// <returns>Имя файла</returns>
        public static string GenerateCanonicalName(string documentType, ImportDeclaration model, DocumentContext documentContext)
        {
            return $"ON_{documentType}_{model.TaxAuthority}_{model.TaxAuthority}_{model.Inn}{model.Kpp}_{documentContext.DateId}_{documentContext.IdFileSuffix}.xml";
        }
    }
}
