using DemoETL.Domain.Models;


namespace DemoETL.Application.Output
{
    public static class FnsFileNameGenerator
    {
        public static string GenerateCanonicalName(string documentType, ImportDeclaration model)
        {
            var date = DateTime.Now.ToString("yyyyMMdd");

            return $"ON_{documentType}_{model.TaxAuthority}_{model.TaxAuthority}_{model.Inn}{model.Kpp}_{date}_{model.IdFileSuffix}.xml";
        }
    }
}
