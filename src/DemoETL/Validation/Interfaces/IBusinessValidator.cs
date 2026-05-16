using DemoETL.Domain.Models;


namespace DemoETL.Validation.Interfaces;

/// <summary>
/// Абстракция бизнес-валидатора
/// 
/// Пример:
/// ПрЛизинг != 1 одновременно с ПрДавСырья != 1
/// По-русски:
/// если ПрДавСырья=1, то ПрЛизинг не может быть равно 1.
/// Если ПрЛизинг=1, то ПрДавСырья не может быть равно 1.
/// </summary>
public interface IBusinessValidator
{
    /// <summary>
    /// Валидируем модель по бизнес логике
    /// </summary>
    /// <param name="model"></param>
    void Validate(ImportDeclaration model);
}
