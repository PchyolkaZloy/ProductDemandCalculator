using Swashbuckle.AspNetCore.Annotations;

namespace DefaultSite;

[SwaggerSchema]
public class WeatherForecast
{
    public DateOnly Date { get; set; }

    [SwaggerSchema("температура в градусах цельсия")]
    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
