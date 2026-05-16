using DemoETL.Application;


namespace DemoETL.Infrastructure
{
    /// <summary>
    /// Парсер CLI аргументов.
    ///
    /// Преобразует:
    ///     argv[]
    ///
    /// в:
    ///     PipelineOptions.
    ///
    /// Поддерживает:
    /// - путь до инпут файла;
    /// - путь до файла на выходе;
    /// - путь до папки с артефактами.
    ///
    /// Изолирует:
    /// - обработку CLI параметров;
    /// - пайплайн.
    /// </summary>
    public static class CommandLineParser
    {   
        /// <summary>
        /// Обрабатываем аргументы из командной строки
        /// </summary>
        /// <param name="args">Массив аргументов (argv)</param>
        /// <returns>Репрезентацию аргументов из командной строки</returns>
        /// <exception cref="Exception">Падаем, если инпут пустой</exception>
        public static PipelineOptions Parse(string[] args)
        {
            if (args.Length == 0)
                throw new Exception("Input file required");

            var options = new PipelineOptions
            {
                InputFile = args[0]
            };

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-o":
                        options.OutputFile = args[++i];
                        break;

                    case "--debug-dir":
                        options.DebugDirectory = args[++i];
                        break;
                }
            }

            return options;
        }
    }
}
