using Microsoft.Extensions.DependencyInjection;
using PedroTer7.MagicShelf.Api.Data.Config.Mappings;
using PedroTer7.MagicShelf.Api.Data.Repositories.Implementations;
using PedroTer7.MagicShelf.Api.Data.Repositories.Interfaces;
using PedroTer7.MagicShelf.Api.Service.Config.Mappings;
using PedroTer7.MagicShelf.Api.Service.Services.Implementations;
using PedroTer7.MagicShelf.Api.Service.Services.Interfaces;

namespace PedroTer7.MagicShelf.Api.CrossCutting
{
    public class CrossCuttingDIConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            AddDataLayerServices(services);
            AddServiceLayerServices(services);
        }

        private static void AddServiceLayerServices(IServiceCollection services)
        {
            services.AddScoped<IItemsService, ItemService>();
        }

        private static void AddDataLayerServices(IServiceCollection services)
        {
            services.AddScoped<IItemRepository, ItemRepository>();
        }

        public static void ConfigureAutoMapperMappingProfiles(IServiceCollection services)
        {
            ConfigureServiceLayerAutoMapperMappingProfiles(services);
            ConfigureDataLayerAutoMapperMappingProfiles(services);
        }

        private static void ConfigureServiceLayerAutoMapperMappingProfiles(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DataToServiceMappingProfile));
            services.AddAutoMapper(typeof(ServiceToDataMappingProfile));
        }

        private static void ConfigureDataLayerAutoMapperMappingProfiles(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DtoToEntityMappingProfile));
            services.AddAutoMapper(typeof(EntityToDtoMappingProfile));
        }
    }
}