using DemoETL.Application;
using DemoETL.Application.Detection;
using DemoETL.Application.Detection.Interfaces;
using DemoETL.Application.Enrichment;
using DemoETL.Application.Enrichment.Interfaces;
using DemoETL.Extraction;
using DemoETL.Extraction.Configuration;
using DemoETL.Extraction.Extractors;
using DemoETL.Extraction.Interfaces;
using DemoETL.Extraction.Parsers;
using DemoETL.Mapping;
using DemoETL.Transformation.Builders;
using DemoETL.Transformation.Transformers;
using DemoETL.Validation.Interfaces;
using DemoETL.Validation.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace DemoETL.Infrastructure
{
    /// <summary>
    /// DI.
    ///
    /// Регистрирует:
    /// - пайплайн;
    /// - сервисы определения типа дока;
    /// - сервисы извлечения данных;
    /// - enrichment сервисы; 
    /// - трансформеры;
    /// - валидаторы.
    ///
    /// Позволяет:
    /// - избежать DI регистраций в Program.cs;
    /// - упростить bootstrap;
    /// - упростить поддержку кода.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрируем сервисы в DI
        /// </summary>
        /// <param name="services">Коллекция сервисов для регистрации в DI</param>
        /// <returns>Коллекция сервисов для регистрации в DI</returns>
        public static IServiceCollection AddDemoEtl(this IServiceCollection services)
        {
            services.AddLogging(cfg => cfg.AddConsole());

            // оркестрация, определение типа документа
            services.AddTransient<Pipeline>();
            services.AddTransient<IDocumentTypeDetector, ConfigDocumentTypeDetector>();

            // извлечение из raw
            services.AddTransient<ExcelReader>();
            services.AddTransient<ExtractionConfigProvider>();
            services.AddTransient<SimpleExtractor>();
            services.AddTransient<TableExtractor>();

            // парсинг, извлечение данных в соответствии с типом документа
            services.AddTransient<IDocumentParser, ZVLRPOKParser>();

            // маппинг, извлечённые данные перекладываем в нашу модель
            services.AddTransient<Mapper>();

            // enrichment модели
            services.AddTransient<IDataEnricher, FnsMetadataEnricher>();
            services.AddTransient<IDataEnricher, OkeiEnricher>();

            // трансформация данных, наша модель -> наш нормализованный XML -> XSLT -> XML по XSD/приказу/ФНС
            services.AddTransient<XmlBuilder>();
            services.AddTransient<XsltTransformer>();

            // валидация
            services.AddTransient<IXmlValidator, XsdValidator>();
            services.AddTransient<IBusinessValidator, ImportDeclarationBusinessValidator>();

            return services;
        }
    }
}
