
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using WebApplication2.Dao;

public class WeatherForecastValidator: AbstractValidator<WeatherForecast> 
{
    public WeatherForecastValidator()
    {
        RuleFor(x=> x.Summary).NotNull().NotEmpty();
        RuleFor(x=> x.TemperatureC).LessThanOrEqualTo(23);
        RuleFor(x=> x.Data).NotEmpty();
        RuleFor(x=>x.Data.ItemData).NotEmpty();
    }
}