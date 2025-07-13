using Microsoft.Extensions.DependencyInjection;
using MsCore.Framework.Utilities.Interfaces;
using MsCore.Framework.Utilities.Providers;

namespace MsCore.Framework.Utilities.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// IServiceCollection'a HTTP client desteği ile MsHttpHelper servisini ekler.
        /// IMsHttpHelper arayüzü için MsHttpHelper implementasyonunu dependency injection container'a kaydeder.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMsCoreHttpHelper(this IServiceCollection services) 
        { 
            services.AddHttpClient<IMsHttpHelper, MsHttpHelper>();
            return services;
        }
    }
}
