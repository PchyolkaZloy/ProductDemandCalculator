namespace Sales.Presentation.Console.Config;

public sealed class AppSettingsConfig
{
    public static string ConfigKey => "AppSettings";
    public int ParallelismDegree { get; set; }

    public int ChannelWriterCapacityInMb { get; set; }
    public int ChannelReaderCapacityInMb { get; set; }

    public string InputFilePath { get; set; }
    public string OutputFilePath { get; set; }
}