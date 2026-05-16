using DemoETL.Transformation.Interfaces;
using System.Xml;
using System.Xml.Xsl;


namespace DemoETL.Transformation.Transformers
{
    /// <summary>
    /// Реализация IXsltTransformer.
    ///
    /// Выполняет:
    /// - XSLT трансформацию;
    /// - преобразование промежуточного XML
    ///   в XML целевого формата.
    ///
    /// Использует:
    /// - XslCompiledTransform (.NET).
    ///
    /// В big проде возможно:
    /// - SaxonCS;
    /// - XSLT 3.0 engines.
    /// </summary>
    public class XsltTransformer : IXsltTransformer
    {
        /// <summary>
        /// Применяем XSLT к промежуточному XML
        /// </summary>
        /// <param name="inputXml">Путь к файлу промежуточного XML</param>
        /// <param name="xsltPath">Путь к файлу XSLT</param>
        /// <param name="outputXml">Путь XML-файла на выходе</param>
        public void Transform(
            string inputXml,
            string xsltPath,
            string outputXml)
        {
            var transform = new XslCompiledTransform();

            transform.Load(xsltPath);

            // пробрасываем динамику, которую не умеет xslt 1.0; или которую не хотим видеть в полностью чистом xslt;
            // для всего остального - есть enrichment слой и модель
            var args = new XsltArgumentList();

            args.AddParam(
                "dateDoc",
                "",
                DateTime.Now.ToString("dd.MM.yyyy"));

            args.AddParam(
                "dateId",
                "",
                DateTime.Now.ToString("yyyyMMdd"));

            using var writer = XmlWriter.Create(
                outputXml,
                transform.OutputSettings);

            transform.Transform(
                inputXml,
                args,
                writer);
        }
    }
}

// Saxon-specific штуки
// ЗЫ: до лучших времён, когда понадобится что-то круче xslt 1.0 и деньги на лицензию
//public class XsltTransformer
//{
//    public void Transform(string inputXml, string xsltPath, string outputXml)
//    {
//        var processor = new Processor();
//        var compiler = processor.NewXsltCompiler();

//        var executable = compiler.Compile(new Uri(xsltPath));
//        var transformer = executable.Load();

//        using var inputStream = File.OpenRead(inputXml);
//        transformer.SetInputStream(inputStream, new Uri(inputXml));

//        using var stream = File.Create(outputXml);
//        var serializer = processor.NewSerializer(stream); // fabric method instead of setter, Saxon's quirk

//        serializer.SetOutputProperty(Serializer.METHOD, "xml");
//        serializer.SetOutputProperty(Serializer.INDENT, "yes"); // fr every single field is QName-string pair, even when you need boolean or int; "under development since 1998" ©
//        serializer.SetOutputProperty(Serializer.ENCODING, "windows-1251");

//        transformer.Run(serializer);
//    }
//}
