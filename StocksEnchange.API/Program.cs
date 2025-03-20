namespace StocksEnchange.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // Configure the HTTP request pipeline.
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
    }
}