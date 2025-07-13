using Microsoft.Extensions.Caching.Memory;

namespace MsCore.Framework.Utilities.Helpers
{
    /// <summary>
    /// Uygulama içi bellek üzerinde cache yönetimi için yardımcı sınıf.
    /// </summary>
    public class MsCacheManager
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Cache Manager yapıcısı. DI üzerinden IMemoryCache alınır.
        /// </summary>
        public MsCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Cache içerisinden ilgili anahtar ile veri getirir.
        /// Veri yoksa null döner.
        /// </summary>
        public T? MsGet<T>(string key)
        {
            return _cache.TryGetValue(key, out T? value) ? value : default;
        }

        /// <summary>
        /// Belirtilen anahtar ve süre ile cache'e veri ekler veya günceller.
        /// </summary>
        public void MsSet<T>(string key, T value, TimeSpan expiration)
        {
            _cache.Set(key, value, expiration);
        }

        /// <summary>
        /// Cache içerisinden ilgili anahtar ile veri silinir.
        /// </summary>
        public void MsRemove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Tüm cache'i temizler.
        /// (MemoryCache implementasyonunda tüm cache'i temizleme doğrudan desteklenmez.
        /// Bu yöntem workaround ile çalışır.)
        /// </summary>
        public void MsClear()
        {
            if (_cache is MemoryCache memCache)
            {
                memCache.Compact(1.0); // Tüm cache'i temizler.
            }
        }
    }
}
