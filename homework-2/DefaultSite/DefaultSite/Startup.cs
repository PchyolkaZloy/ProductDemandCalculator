using FluentValidation;
using Infrastructure;
using Microsoft.Extensions.Options;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c=>{
            c.EnableAnnotations();
        });

        services.Configure<DefaultValueConfiguration>(_configuration.GetSection(nameof(DefaultValueConfiguration)));
        
        services.Configure<MyOptions>(op=>{
            op.DefaultName = "teste";
        });

        services.AddGrpc(op=>{
            op.Interceptors.Add<LoggerInterceptor>();
            op.Interceptors.Add<ExceptionInterceptor>();
        });
        
        services.AddGrpcReflection();

        services.AddSingleton<IGoodsRepository, GoodsRepository>();
        services.AddScoped<CurrentMiddleware>();

        services.AddValidatorsFromAssemblyContaining<GoodItemValidator>();
        services.AddHostedService<MyBackgroundService>();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseMiddleware<LoggerMiddleware>();
        app.UseMiddleware<CurrentMiddleware>();
      
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.UseDeveloperExceptionPage(); - если включить то не будет работать переопрделения ошибок в других слоях middleware

        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
           endpoints.MapGrpcService<GreeterService>();
           //endpoints.MapGrpcReflectionService();
        });
    }
}