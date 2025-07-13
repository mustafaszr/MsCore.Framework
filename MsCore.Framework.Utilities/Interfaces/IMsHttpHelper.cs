using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Utilities.Interfaces
{
    public interface IMsHttpHelper
    {
        /// <summary>
        /// Belirtilen URL'e HTTP GET isteği gönderir.
        /// İsteğe bağlı olarak header bilgileri ve Bearer Token eklenebilir.
        /// Dönen JSON yanıtını belirtilen tipe deserialize eder.
        /// </summary>
        Task<TResponse> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null, string? token = null);
        /// <summary>
        /// Belirtilen URL'e HTTP POST isteği gönderir.
        /// Gönderilecek veri JSON olarak gövdeye eklenir.
        /// İsteğe bağlı olarak header bilgileri ve Bearer Token eklenebilir.
        /// Dönen JSON yanıtı belirtilen tipe deserialize edilir.
        /// </summary>
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null, string? token = null);
        /// <summary>
        /// Belirtilen URL'e HTTP PUT isteği gönderir.
        /// Gönderilecek veri JSON olarak gövdeye eklenir.
        /// İsteğe bağlı olarak header bilgileri ve Bearer Token eklenebilir.
        /// Dönen JSON yanıtı belirtilen tipe deserialize edilir.
        /// </summary>
        Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null, string? token = null);
        /// <summary>
        /// Belirtilen URL'e HTTP DELETE isteği gönderir.
        /// İsteğe bağlı olarak header bilgileri ve Bearer Token eklenebilir.
        /// Dönen JSON yanıtı belirtilen tipe deserialize edilir.
        /// </summary>
        Task<TResponse> DeleteAsync<TResponse>(string url, Dictionary<string, string>? headers = null, string? token = null);
    }
}
