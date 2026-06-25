using UnityEngine;

namespace Ghost.Presentation.Backend
{
    public static class GhostBackendConfig
    {
        public const string DefaultBaseUrl = "http://localhost:3000";
        public const int DefaultRequestTimeoutSeconds = 4;
        public const string BaseUrlPlayerPrefsKey = "Ghost.Backend.BaseUrl";

        private static string runtimeBaseUrl;
        private static int requestTimeoutSeconds = DefaultRequestTimeoutSeconds;

        public static string BaseUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(runtimeBaseUrl))
                {
                    return TrimTrailingSlash(runtimeBaseUrl);
                }

                var savedBaseUrl = PlayerPrefs.GetString(BaseUrlPlayerPrefsKey, DefaultBaseUrl);
                return TrimTrailingSlash(savedBaseUrl);
            }

            set
            {
                runtimeBaseUrl = string.IsNullOrWhiteSpace(value)
                    ? DefaultBaseUrl
                    : value.Trim();

                PlayerPrefs.SetString(BaseUrlPlayerPrefsKey, runtimeBaseUrl);
                PlayerPrefs.Save();
            }
        }

        public static int RequestTimeoutSeconds
        {
            get => Mathf.Clamp(requestTimeoutSeconds, 1, 30);
            set => requestTimeoutSeconds = Mathf.Clamp(value, 1, 30);
        }

        public static string BuildUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return BaseUrl;
            }

            if (path.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            var normalizedPath = path.StartsWith("/", System.StringComparison.Ordinal)
                ? path
                : "/" + path;

            return BaseUrl + normalizedPath;
        }

        private static string TrimTrailingSlash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DefaultBaseUrl;
            }

            return value.Trim().TrimEnd('/');
        }
    }
}
