namespace WebApplication2.Dao;

public class WeatherForecast
{
    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    public TEstData Data{get;set;}
}


public record TEstData
{
    public string ItemData{get;set;}
}
