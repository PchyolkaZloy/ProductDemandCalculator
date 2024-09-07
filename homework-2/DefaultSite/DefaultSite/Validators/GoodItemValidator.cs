using FluentValidation;

public class GoodItemValidator:AbstractValidator<GoodItem>
{
    public GoodItemValidator()
    {
        RuleFor(x=> x.Price).GreaterThan(0);
    }
}