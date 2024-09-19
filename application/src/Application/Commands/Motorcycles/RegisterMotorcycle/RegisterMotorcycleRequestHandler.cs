using Domain.Common.Interfaces;
using FluentValidation;
using MediatR;
using Domain.Motorcycle;
using Domain.Common.Implementations;
using Application.Interfaces;

namespace Application.Commands.Motorcycles.RegisterMotorcycle;

public class RegisterMotorcycleResponse
{
    public Guid MotorcycleId {get; set;}
    public string LicensePlate {get; set;}
}

public class RegisterMotorcycleRequest : IRequest<IResult<RegisterMotorcycleResponse>>
{
    public int Year {get; set;}
    public string Model {get; set;}
    public string LicensePlate {get; set;}
}

public class RegisterMotorcycleRequestValidator : AbstractValidator<RegisterMotorcycleRequest>
{
    public RegisterMotorcycleRequestValidator()
    {
        RuleFor(x => x.Year).NotNull().WithMessage("Year is required");
        RuleFor(x => x.Model).NotNull().WithMessage("Model is required");
        RuleFor(x => x.LicensePlate).NotNull().WithMessage("License plate is required");
    }    
}

public class RegisterMotorcycleRequestHandler : IRequestHandler<RegisterMotorcycleRequest, IResult<RegisterMotorcycleResponse>>
{
    private readonly IMotorcycleRepository _motorcyleRepository;
    private readonly IEventPublisher _eventPublisher;
    public RegisterMotorcycleRequestHandler(IMotorcycleRepository motorcycleRepository, IEventPublisher eventPublisher)
    {
        _motorcyleRepository = motorcycleRepository;
        _eventPublisher = eventPublisher;
    }
    public async Task<IResult<RegisterMotorcycleResponse>> Handle(RegisterMotorcycleRequest request, CancellationToken cancellationToken)
    {
        List<string> errors = new();

        var motorcycle = new Motorcycle(request.Year, request.Model, request.LicensePlate);
        var motorcycleValidationResult = motorcycle.Validate();

        IResult<bool> publishedEvent = await _eventPublisher.PublishAsync(motorcycle);

        if(!motorcycleValidationResult.IsSuccess)
        {
            errors.AddRange(motorcycleValidationResult.Errors);
        }

        if(errors.Any())
        {
            return Result<RegisterMotorcycleResponse>.Fail(errors);
        }

        IResult<Motorcycle?> motorcycleExists = await _motorcyleRepository.GetByLicensePlateAsync(request.LicensePlate);
        if(motorcycleExists.IsSuccess){
            errors.Add("The license plate is already registered");
        }

        if(errors.Any())
        {
            return Result<RegisterMotorcycleResponse>.Fail(errors);
        }

        IResult<bool> motorcycleAdded = await _motorcyleRepository.AddAsync(motorcycle);
        if(!motorcycleAdded.IsSuccess){
            return Result<RegisterMotorcycleResponse>.Fail(motorcycleAdded.Errors.ToList());
        }

        //TODO: Trigger event/Call RabbitMQ ... 
        // IResult<bool> publishedEvent = await _eventPublisher.PublishAsync(motorcycle);
        // Fazer log se não publicou o event...
        
        return Result<RegisterMotorcycleResponse>.Success(new RegisterMotorcycleResponse { MotorcycleId = motorcycle.MotorcyleId, LicensePlate = motorcycle.LicensePlate });
    }
}