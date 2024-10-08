using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Users.RegisterDeliveryDriver;

public class RegisterDeliveryDriverRequestValidator : AbstractValidator<RegisterDeliveryDriverRequest>
{
    public RegisterDeliveryDriverRequestValidator()
    {
        // Validação do Username
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Length(5, 50).WithMessage("Username must be between 5 and 50 characters.");

        // Validação da Senha
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        // Validação do CNPJ
        RuleFor(x => x.BusinessIdentificationNumber)
            .NotEmpty().WithMessage("CNPJ is required.");

        // Validação da CNH
        RuleFor(x => x.DriverLicenseNumber)
            .NotEmpty().WithMessage("CNH is required");

        // Validação do Nome
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

        // Validação da Data de Nascimento (motorista deve ter no mínimo 18 anos)
        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("BirthDate is required.");
    }
}