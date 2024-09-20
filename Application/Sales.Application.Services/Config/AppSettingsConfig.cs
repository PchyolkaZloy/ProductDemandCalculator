namespace Sales.Application.Services.Config;

public sealed class AppSettingsConfig
{
    public static string ConfigKey => "AppSettings";
    public int ParallelismDegree { get; set; }
    public string InputFilePath { get; set; }
    public string OutputFilePath { get; set; }
}