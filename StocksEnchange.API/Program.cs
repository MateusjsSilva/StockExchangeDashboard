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
            });
            
            builder.Services.AddHostedService<Services.StockService>();
            builder.Services.AddSingleton<Services.StockService>();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            
            app.UseStaticFiles();
            
            app.UseCors("AllowAll");
            
            app.MapControllers();
            app.MapHub<Hubs.StockHub>("/stockHub");
            
            app.MapGet("/", async context =>
            {
                context.Response.Redirect("/index.html");
            });

            app.Run();
        }
    }
}