using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MsCore.Framework.Logging.Interfaces;
using MsCore.Framework.Logging.Models.LoggingConfiguration;
using MsCore.Framework.Logging.Providers;

namespace MsCore.Framework.Logging.Extensions
{
    /// <summary>
    /// MsCore Logger servislerini bağımlılık enjeksiyonuna (DI) eklemek için genişletme metodlarını sağlar.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// MsCore Logger'ı dosya tabanlı, veritabanı tabanlı veya her ikisini destekleyecek şekilde yapılandırarak
        /// bağımlılık enjeksiyonuna (DI) kaydeder.
        /// </summary>
        /// <param name="services">Servislerin ekleneceği IServiceCollection nesnesi.</param>
        /// <param name="configure">
        /// <see cref="LoggerOptions"/> yapılandırmasını sağlayan bir Action delegate.
        /// Dosya veya veritabanı logger'larının aktif edilmesi, klasör yolu, dosya adı, dosya döngüleme kuralları ve dosya boyutu gibi ayarları içerir.
        /// </param>
        /// <returns>Logger'ların kayıtlı olduğu IServiceCollection nesnesi.</returns>
        /// <example>
        /// services.AddMsCoreLogger(options =>
        /// {
        ///     options.UseFileLogger = true;
        ///     options.DirectoryPath = "C:\\Logs";
        ///     options.FileName = "applog";
        ///     options.MaxFileSizeInMB = 10;
        ///     options.RotationType = RotationType.Daily;
        ///     options.UseDatabaseLogger = true;
        /// });
        /// </example>
        public static IServiceCollection AddMsCoreLogger<TContext>(this IServiceCollection services, Action<LoggerOptions> configure) where TContext : DbContext
        {
            var options = new LoggerOptions();
            configure(options);

            var loggerList = new List<IMsDbLoggerService>();

            if (options.UseFileLogger)
            {
                services.AddScoped<IMsFileLoggerService, MsFileLogger>();
                services.Configure<FileLoggerOptions>(opt =>
                {
                    opt.DirectoryPath = options.DirectoryPath;
                    opt.FileName = options.FileName;
                    opt.MaxFileSizeInMB = options.MaxFileSizeInMB;
                    opt.RotationType = options.RotationType;
                });
            }

            if (options.UseDatabaseLogger)
            {
                services.AddScoped<IMsDbLoggerService, MsDbLogger<TContext>>();
            }

            services.AddScoped<IMsLoggerService, MsCompositeLogger>();
            return services;
        }
    }
}
