using Compass.Data.Data.Classes;
using Compass.Data.Data.Interfaces;

namespace Compass.API.Infrastructure.Repositories
{
    public class RepositoriesConfiguration
    {
        public static void Config(IServiceCollection services)
        {
            // Add IUserRepository
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
