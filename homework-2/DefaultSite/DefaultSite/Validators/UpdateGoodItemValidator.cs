using FluentValidation;

public class UpdateGoodItemValidator:AbstractValidator<UpdateGoodItem>
{
    public UpdateGoodItemValidator()
    {
        RuleFor(x=> x.Summary).NotEmpty();
    }
}