using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Evacuation.Core.Interfaces.Infrastructure.Database;
using Evacuation.Infrastructure.Caching;
using Evacuation.Infrastructure.Database;
using Evacuation.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Evacuation.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<DataContext>(opt =>
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Redis Cache
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("Redis");
            });

            services.AddScoped<ICacheService, CacheService>();

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEvacuationPlanRepository, EvacuationPlanRepository>();
            services.AddScoped<IEvacuationZoneRepository, EvacuationZoneRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            return services;
        }
    }
}
