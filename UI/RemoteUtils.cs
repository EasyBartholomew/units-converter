using System.Net;
using System.Threading.Tasks;

namespace UI
{
    /// <summary>
    /// Представляет набор вспомогательных методов для работы с сетью и сервером RG
    /// </summary>
    public static class RemoteUtils
    {
        /// <summary>
        /// Синхронно проверяет подключение к сети интернет
        /// </summary>
        /// <param name="timeoutMs">Время ожидания</param>
        /// <param name="url">Адрес для проверки соединения</param>
        /// <returns>true, если подключение есть, иначе - false</returns>
        public static bool CheckForInternetConnection(int timeoutMs = 1000, string url = null)
        {
            try
            {
                if (url == null)
                    url = "http://www.gstatic.com/generate_204";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using ((HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Асинхронно проверяет подключение к сети интернет
        /// </summary>
        /// <param name="timeoutMs">Время ожидания</param>
        /// <param name="url">Адрес для проверки соединения</param>
        /// <returns>Возвращает задачу проверки подключения</returns>
        public static Task<bool> CheckForInternetConnectionAsync(int timeoutMs = 1000, string url = null) =>
            Task.Run(() => CheckForInternetConnection(timeoutMs, url));
    }
}
