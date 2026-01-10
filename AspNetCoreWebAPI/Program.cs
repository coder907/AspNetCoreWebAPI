using System.Reflection;
using AspNetCoreWebAPI.Interfaces;
using AspNetCoreWebAPI.Repositories;

namespace AspNetCoreWebAPI
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigureRequestPipeline(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();

            // Configure Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "AspNetCore Web API",
                    Version = "v1",
                    Description = "A sample ASP.NET Core Web API with Category and Product models"
                });

                // Include XML comments for better documentation
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                options.IncludeXmlComments(xmlPath);
            });

            // Register repository using factory method (can be swapped with different implementations)
            services.AddScoped<IProductRepository>(serviceProvider => 
                CreateProductRepository(serviceProvider, configuration));
        }

        private static IProductRepository CreateProductRepository(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // Factory method to create repository instance
            // Can add logic here to select different implementations based on configuration
            
            // Example: You could switch implementations based on configuration:
            // var repositoryType = configuration["RepositoryType"];
            // return repositoryType switch
            // {
            //     "Database" => new DatabaseProductRepository(serviceProvider.GetRequiredService<DbContext>()),
            //     "Cache" => new CachedProductRepository(serviceProvider.GetRequiredService<IMemoryCache>()),
            //     "InMemory" => new InMemoryProductRepository(),
            //     _ => new InMemoryProductRepository()
            // };
            
            return new InMemoryProductRepository();
        }

        private static void ConfigureRequestPipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCore Web API v1");
                    options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
