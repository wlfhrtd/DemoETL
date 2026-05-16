namespace DemoETL.Application
{
    /// <summary>
    /// Рантайм опции пайплайна.
    ///
    /// Формируется:
    /// - из CLI аргументов;
    /// - CommandLineParser.
    ///
    /// Позволяет:
    /// - задавать input/output;
    /// - управлять debug artifacts;
    /// - управлять intermediate xml.
    ///
    /// Используется:
    /// - Pipeline.
    ///
    /// Пример:
    ///
    ///     DemoETL.exe input.xlsx
    ///
    ///     DemoETL.exe input.xlsx -o result.xml
    ///
    ///     DemoETL.exe input.xlsx --debug-dir artifacts
    /// </summary>
    public class PipelineOptions
    {
        /// <summary>
        /// Путь до инпута
        /// </summary>
        public string InputFile { get; set; } = string.Empty;

        /// <summary>
        /// Путь для сохранения данных
        /// </summary>
        public string? OutputFile { get; set; }

        /// <summary>
        /// Папка для сохранения наших нормализованных промежуточных xml
        /// </summary>
        public string? DebugDirectory { get; set; }

        /// <summary>
        /// Флаг - храним промежуточные xml в DebugDirectory или %temp%
        /// </summary>
        public bool KeepArtifacts =>
            !string.IsNullOrWhiteSpace(DebugDirectory);
    }
}
