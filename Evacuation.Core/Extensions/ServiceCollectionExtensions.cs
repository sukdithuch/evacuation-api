using Evacuation.Core.Interfaces.Services;
using Evacuation.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evacuation.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreService(this IServiceCollection services)
        {
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IEvacuationZoneService, EvacuationZoneService>();
            services.AddScoped<IEvacuationPlanService, EvacuationPlanService>();
            services.AddScoped<IEvacuationStatusService, EvacuationStatusService>();

            return services;
        }
    }
}
