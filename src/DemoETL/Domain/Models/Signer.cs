namespace DemoETL.Domain.Models;

/// <summary>
/// Подписант
/// </summary>
public class Signer
{
    /// <summary>
    /// Тип подписанта.
    /// Пример:
    /// 1 - индивидуальный предприниматель |
    /// 2 - руководитель организации |
    /// 3 - уполномоченный представитель
    /// </summary>
    public int SignerType { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Отчество
    /// </summary>
    public string? MiddleName { get; set; }
}
