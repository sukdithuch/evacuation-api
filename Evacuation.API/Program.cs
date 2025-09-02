using Evacuation.Core.Extensions;
using Evacuation.Core.Mappings;
using Evacuation.Infrastructure.Extensions;

namespace Evacuation.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Infrastructure DI
            builder.Services.AddInfrastructureService(builder.Configuration);

            // Core DI
            builder.Services.AddCoreService();

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            Infrastructure.Extensions.ServiceCollectionExtensions.ApplyMigration(app.Services);

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
