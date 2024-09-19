using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CitiesManager.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            //Swagger
            builder.Services.AddEndpointsApiExplorer(); //Generates description for all endpoints
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(Path.Combine
                    (AppContext.BaseDirectory, "api.xml"));
            }); //generates OpenAPI specification

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
                logging.RequestHeaders.Add("sec-ch-ua");
                logging.ResponseHeaders.Add("MyResponseHeader");
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.CombineLogs = true;
            });

            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseSwagger(); //creates endpoint for swagger.json
            app.UseSwaggerUI(); //creates swagger UI for testing all Web API endpoints / action methods

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
