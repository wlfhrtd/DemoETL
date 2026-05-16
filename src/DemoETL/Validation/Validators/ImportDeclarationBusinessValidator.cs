using DemoETL.Domain.Models;
using DemoETL.Validation.Interfaces;


namespace DemoETL.Validation.Validators;

/// <summary>
/// Бизнес-валидатор
/// 
/// Пример:
/// ПрЛизинг != 1 одновременно с ПрДавСырья != 1
/// По-русски:
/// если ПрДавСырья=1, то ПрЛизинг не может быть равно 1.
/// Если ПрЛизинг=1, то ПрДавСырья не может быть равно 1.
/// </summary>
public class ImportDeclarationBusinessValidator
    : IBusinessValidator
{
    /// <summary>
    /// Валидируем нашу наполненную модель
    /// </summary>
    /// <param name="model">Модель</param>
    /// <exception cref="Exception">Падаем, если не проходим по приказу ФНС</exception>
    public void Validate(ImportDeclaration model)
    {
        if (model.IsLeasing && model.IsTolling)
        {
            throw new Exception(
                "Leasing and tolling flags " +
                "cannot both equal true.");
        }
    }
}
