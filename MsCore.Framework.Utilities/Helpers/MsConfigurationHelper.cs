using Microsoft.Extensions.Configuration;

namespace MsCore.Framework.Utilities.Helpers
{
    /// <summary>
    /// Uygulama konfigürasyon ayarlarını yönetmek için yardımcı sınıf.
    /// </summary>
    public static class MsConfigurationHelper
    {
        /// <summary>
        /// IConfiguration nesnesi.
        /// Program.cs → builder.Configuration üzerinden set edilmeli.
        /// → Helper'a Configuration'ı set et => ConfigurationHelper.Initialize(builder.Configuration);
        /// </summary>
        public static IConfiguration? Configuration { get; private set; }

        /// <summary>
        /// IConfiguration'ı initialize eder.
        /// Program.cs → builder.Configuration gönderilmeli.
        /// </summary>
        public static void MsInitialize(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Connection string bilgisini getirir.
        /// </summary>
        public static string MsGetConnectionString(string name)
        {
            MsCheckIsInitialized();
            var value = Configuration!.GetConnectionString(name);
            if (string.IsNullOrEmpty(value))
                throw new Exception($"Connection string '{name}' not found in configuration.");
            return value;
        }

        /// <summary>
        /// Belirtilen key için konfigürasyon değerini getirir.
        /// </summary>
        public static T MsGetValue<T>(string key, T? defaultValue = default)
        {
            MsCheckIsInitialized();
            var value = Configuration!.GetValue<T>(key);
            return value != null && !value.Equals(default(T)) ? value : defaultValue!;
        }

        /// <summary>
        /// Belirtilen section'ı getirir.
        /// </summary>
        public static IConfigurationSection MsGetSection(string sectionName)
        {
            MsCheckIsInitialized();
            return Configuration!.GetSection(sectionName);
        }

        /// <summary>
        /// Section'ı direkt olarak bir class'a bind eder.
        /// </summary>
        public static T MsGetSectionAs<T>(string sectionName) where T : new()
        {
            MsCheckIsInitialized();
            var section = new T();
            Configuration!.GetSection(sectionName).Bind(section);
            return section;
        }

        /// <summary>
        /// Bulunamazsa exception fırlatan zorunlu key okuma.
        /// </summary>
        public static T MsGetRequiredValue<T>(string key)
        {
            MsCheckIsInitialized();
            var value = Configuration!.GetValue<T>(key);
            if (value == null || value.Equals(default(T)))
                throw new Exception($"Configuration key '{key}' not found or empty.");
            return value;
        }

        /// <summary>
        /// Çalışılan environment bilgisini getirir. (Development, Production vs.)
        /// </summary>
        public static string MsGetEnvironment()
        {
            return MsGetValue<string>("ASPNETCORE_ENVIRONMENT", "Production");
        }

        /// <summary>
        /// Şu anda Development ortamında mı çalışıyor?
        /// </summary>
        public static bool MsIsDevelopment()
        {
            var env = MsGetEnvironment();
            return env.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Şu anda Production ortamında mı çalışıyor?
        /// </summary>
        public static bool MsIsProduction()
        {
            var env = MsGetEnvironment();
            return env.Equals("Production", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Şu anda Staging ortamında mı çalışıyor?
        /// </summary>
        public static bool MsIsStaging()
        {
            var env = MsGetEnvironment();
            return env.Equals("Staging", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Configuration yüklenmiş mi kontrol eder.
        /// </summary>
        private static void MsCheckIsInitialized()
        {
            if (Configuration == null)
                throw new InvalidOperationException("Configuration is not initialized. Call ConfigurationHelper.Initialize(builder.Configuration) first.");
        }
    }
}
