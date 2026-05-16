using System.Text;


namespace DemoETL.Infrastructure
{
    /// <summary>
    /// Бутстрап провайдеров кодировок.
    ///
    /// Необходим для:
    /// - Windows-1251;
    /// - legacy кодировок;
    /// - XML/XSLT совместимости.
    ///
    /// Используется:
    /// - при старте приложения.
    /// </summary>
    public static class EncodingBootstrap
    {
        /// <summary>
        /// Загружаем провайдер кодировок.
        /// </summary>
        public static void Register()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
