using FluentValidation;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.DestinationId).NotEmpty().WithMessage("DestinationId is required.");
    }
}