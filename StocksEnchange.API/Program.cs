using Serilog;

namespace StocksEnchange.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services));

            try
            {
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ConfigureEndpointDefaults(listenOptions => { });
                })
                .UseUrls("http://localhost:5000", "https://localhost:5001");

                // Service configuration
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // SignalR
                builder.Services.AddSignalR(options =>
                {
                    options.MaximumReceiveMessageSize = 102400;
                    options.StreamBufferCapacity = 20;
                    options.EnableDetailedErrors = true;
                });
                
                builder.Services.AddHostedService<Services.StockService>();
                builder.Services.AddSingleton<Services.StockService>();
                
                // CORS configuration
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });

                    options.AddPolicy("CorsPolicy", builder =>
                    {
                        builder
                            .WithOrigins("https://localhost:5001", "https://localhost:7001", 
                                        "http://localhost:5000", "http://localhost:7000")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                });

                var app = builder.Build();

                Log.Information("Web server configured to listen on: http://localhost:5000, https://localhost:5001");

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    app.UseDeveloperExceptionPage();
                }

                app.UseHttpsRedirection();
                app.UseRouting();
                
                app.UseCors();
                
                app.UseAuthorization();
                app.UseStaticFiles();
                app.MapControllers();

                app.MapHub<Hubs.StockHub>("/stockHub");
                
                app.MapGet("/", async context =>
                {
                    await Task.Run(() => context.Response.Redirect("/index.html"));
                });

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}