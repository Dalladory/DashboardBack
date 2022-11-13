using Compass.Data.Data.AutoMapper;

namespace Compass.API.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Config(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperUserProfile));
        }
    }
}
