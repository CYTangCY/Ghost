using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ghost.Presentation.Shell;
using UnityEngine;
using UnityEngine.Networking;

namespace Ghost.Presentation.Backend
{
    public static class GhostBackendClient
    {
        public const string CorrectResult = "correct";
        public const string IncorrectResult = "incorrect";

        private const string RunnerName = "Ghost Backend Client Runner";
        private const int LlmRequestTimeoutSeconds = 65;

        private static GhostBackendClientRunner runner;
        private static bool isEnsuringProfile;
        private static readonly List<Action<GhostBackendResponse<ProfileResponse>>> PendingProfileCallbacks =
            new List<Action<GhostBackendResponse<ProfileResponse>>>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRunner()
        {
            EnsureRunner();
        }

        public static void EnsureProfile(Action<GhostBackendResponse<ProfileResponse>> callback = null)
        {
            var existingProfileId = GhostNarrativeState.BackendProfileId;
            if (!string.IsNullOrWhiteSpace(existingProfileId))
            {
                callback?.Invoke(GhostBackendResponse<ProfileResponse>.Success(
                    new ProfileResponse { id = existingProfileId },
                    0));
                return;
            }

            if (callback != null)
            {
                PendingProfileCallbacks.Add(callback);
            }

            if (isEnsuringProfile)
            {
                return;
            }

            isEnsuringProfile = true;
            Run(SendRequest("POST", "/profiles", "{}", (GhostBackendResponse<ProfileResponse> response) =>
            {
                isEnsuringProfile = false;

                if (response.Succeeded && response.Value != null && !string.IsNullOrWhiteSpace(response.Value.id))
                {
                    GhostNarrativeState.SetBackendProfileId(response.Value.id);
                }

                FlushPendingProfileCallbacks(response);
            }));
        }

        public static void GetProgress(
            string profileId,
            Action<GhostBackendResponse<ProgressResponse>> callback)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                callback?.Invoke(GhostBackendResponse<ProgressResponse>.Failed("No backend profile id is available."));
                return;
            }

            Run(SendRequest(
                "GET",
                "/progress/" + UnityWebRequest.EscapeURL(profileId),
                null,
                callback));
        }

        public static void PutProgress(
            ProgressRequest progress,
            Action<GhostBackendResponse<ProgressResponse>> callback = null)
        {
            EnsureProfile(profileResponse =>
            {
                if (!profileResponse.Succeeded || profileResponse.Value == null)
                {
                    callback?.Invoke(GhostBackendResponse<ProgressResponse>.Failed("No backend profile id is available."));
                    return;
                }

                Run(SendRequest(
                    "PUT",
                    "/progress/" + UnityWebRequest.EscapeURL(profileResponse.Value.id),
                    JsonUtility.ToJson(progress ?? new ProgressRequest()),
                    callback));
            });
        }

        public static void PostAttempt(
            string actId,
            string result,
            AttemptDetails details = null,
            Action<GhostBackendResponse<AttemptResponse>> callback = null)
        {
            if (string.IsNullOrWhiteSpace(actId) || string.IsNullOrWhiteSpace(result))
            {
                callback?.Invoke(GhostBackendResponse<AttemptResponse>.Failed("Attempt act id and result are required."));
                return;
            }

            EnsureProfile(profileResponse =>
            {
                if (!profileResponse.Succeeded || profileResponse.Value == null)
                {
                    callback?.Invoke(GhostBackendResponse<AttemptResponse>.Failed("No backend profile id is available."));
                    return;
                }

                var payload = new AttemptRequest
                {
                    profileId = profileResponse.Value.id,
                    actId = actId,
                    result = result,
                    details = details ?? new AttemptDetails()
                };

                Run(SendRequest(
                    "POST",
                    "/attempts",
                    JsonUtility.ToJson(payload),
                    callback));
            });
        }

        public static void PostHint(
            string actId,
            string trigger,
            string stateSummary,
            Action<GhostBackendResponse<HintResponse>> callback = null)
        {
            if (string.IsNullOrWhiteSpace(actId))
            {
                callback?.Invoke(GhostBackendResponse<HintResponse>.Failed("Hint act id is required."));
                return;
            }

            EnsureProfile(profileResponse =>
            {
                if (!profileResponse.Succeeded || profileResponse.Value == null)
                {
                    callback?.Invoke(GhostBackendResponse<HintResponse>.Failed("No backend profile id is available."));
                    return;
                }

                var payload = new HintRequest
                {
                    profileId = profileResponse.Value.id,
                    actId = actId,
                    level = "1",
                    trigger = string.IsNullOrWhiteSpace(trigger) ? "unspecified" : trigger,
                    state = new HintState
                    {
                        summary = stateSummary ?? string.Empty
                    }
                };

                Run(SendRequest(
                    "POST",
                    "/hints",
                    JsonUtility.ToJson(payload),
                    callback,
                    LlmRequestTimeoutSeconds));
            });
        }

        public static void PostResponse(
            string actId,
            string stateSummary,
            Action<GhostBackendResponse<GeneratedResponse>> callback = null)
        {
            if (string.IsNullOrWhiteSpace(actId))
            {
                callback?.Invoke(GhostBackendResponse<GeneratedResponse>.Failed("Response act id is required."));
                return;
            }

            EnsureProfile(profileResponse =>
            {
                if (!profileResponse.Succeeded || profileResponse.Value == null)
                {
                    callback?.Invoke(GhostBackendResponse<GeneratedResponse>.Failed("No backend profile id is available."));
                    return;
                }

                var payload = new HintRequest
                {
                    profileId = profileResponse.Value.id,
                    actId = actId,
                    level = "1",
                    state = new HintState
                    {
                        summary = stateSummary ?? string.Empty
                    }
                };

                Run(SendRequest(
                    "POST",
                    "/responses",
                    JsonUtility.ToJson(payload),
                    callback,
                    LlmRequestTimeoutSeconds));
            });
        }

        public static string CreateAttemptResult(bool isCorrect)
        {
            return isCorrect ? CorrectResult : IncorrectResult;
        }

        public static AttemptDetails CreateAttemptDetails(
            string source,
            IReadOnlyList<string> errors,
            string summary = null)
        {
            return new AttemptDetails
            {
                source = string.IsNullOrWhiteSpace(source) ? "unity-client" : source,
                summary = summary ?? string.Empty,
                errorCount = errors == null ? 0 : errors.Count,
                errors = CopyErrors(errors)
            };
        }

        private static string[] CopyErrors(IReadOnlyList<string> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return new string[0];
            }

            var copiedErrors = new string[errors.Count];
            for (var i = 0; i < errors.Count; i++)
            {
                copiedErrors[i] = errors[i] ?? string.Empty;
            }

            return copiedErrors;
        }

        private static IEnumerator SendRequest<T>(
            string method,
            string path,
            string jsonBody,
            Action<GhostBackendResponse<T>> callback,
            int timeoutSeconds = 0)
        {
            var url = GhostBackendConfig.BuildUrl(path);
            using (var request = new UnityWebRequest(url, method))
            {
                request.timeout = timeoutSeconds > 0
                    ? timeoutSeconds
                    : GhostBackendConfig.RequestTimeoutSeconds;
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Accept", "application/json");

                if (jsonBody != null)
                {
                    var bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                    request.uploadHandler = new UploadHandlerRaw(bodyBytes);
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    var message = CreateFailureMessage(method, url, request);
                    Debug.LogWarning("[GhostBackendClient] " + message);
                    callback?.Invoke(GhostBackendResponse<T>.Failed(message, request.responseCode));
                    yield break;
                }

                var responseText = request.downloadHandler == null
                    ? string.Empty
                    : request.downloadHandler.text;

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    callback?.Invoke(GhostBackendResponse<T>.Success(default(T), request.responseCode));
                    yield break;
                }

                T parsedResponse;
                try
                {
                    parsedResponse = JsonUtility.FromJson<T>(responseText);
                }
                catch (Exception exception)
                {
                    var message = "Backend response parse failed for " + method + " " + url + ": " + exception.Message;
                    Debug.LogWarning("[GhostBackendClient] " + message);
                    callback?.Invoke(GhostBackendResponse<T>.Failed(message, request.responseCode));
                    yield break;
                }

                callback?.Invoke(GhostBackendResponse<T>.Success(parsedResponse, request.responseCode));
            }
        }

        private static string CreateFailureMessage(string method, string url, UnityWebRequest request)
        {
            var error = string.IsNullOrWhiteSpace(request.error)
                ? "request failed"
                : request.error;

            return method + " " + url + " failed (" + request.responseCode + "): " + error;
        }

        private static void FlushPendingProfileCallbacks(GhostBackendResponse<ProfileResponse> response)
        {
            var callbacks = PendingProfileCallbacks.ToArray();
            PendingProfileCallbacks.Clear();

            foreach (var callback in callbacks)
            {
                callback?.Invoke(response);
            }
        }

        private static void Run(IEnumerator routine)
        {
            EnsureRunner().StartCoroutine(routine);
        }

        private static GhostBackendClientRunner EnsureRunner()
        {
            if (runner != null)
            {
                return runner;
            }

            var existingRunnerObject = GameObject.Find(RunnerName);
            if (existingRunnerObject != null)
            {
                runner = existingRunnerObject.GetComponent<GhostBackendClientRunner>();
                if (runner != null)
                {
                    return runner;
                }
            }

            var runnerObject = new GameObject(RunnerName);
            runnerObject.hideFlags = HideFlags.HideInHierarchy;
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
            runner = runnerObject.AddComponent<GhostBackendClientRunner>();
            return runner;
        }

        private sealed class GhostBackendClientRunner : MonoBehaviour
        {
        }
    }

    public sealed class GhostBackendResponse<T>
    {
        private GhostBackendResponse(bool succeeded, T value, string error, long statusCode)
        {
            Succeeded = succeeded;
            Value = value;
            Error = error ?? string.Empty;
            StatusCode = statusCode;
        }

        public bool Succeeded { get; }

        public T Value { get; }

        public string Error { get; }

        public long StatusCode { get; }

        public static GhostBackendResponse<T> Success(T value, long statusCode)
        {
            return new GhostBackendResponse<T>(true, value, string.Empty, statusCode);
        }

        public static GhostBackendResponse<T> Failed(string error, long statusCode = 0)
        {
            return new GhostBackendResponse<T>(false, default(T), error, statusCode);
        }
    }

    [Serializable]
    public sealed class ProfileResponse
    {
        public string id;
        public string createdAt;
    }

    [Serializable]
    public sealed class ProgressResponse
    {
        public string profileId;
        public string[] actsCompleted;
        public string[] levelsCompleted;
        public NarrativeStatePayload narrativeState;
        public string updatedAt;
    }

    [Serializable]
    public sealed class ProgressRequest
    {
        public string[] actsCompleted = new string[0];
        public string[] levelsCompleted = new string[0];
        public NarrativeStatePayload narrativeState = new NarrativeStatePayload();
    }

    [Serializable]
    public sealed class NarrativeStatePayload
    {
        public string playerName;
    }

    [Serializable]
    public sealed class AttemptRequest
    {
        public string profileId;
        public string actId;
        public string result;
        public AttemptDetails details = new AttemptDetails();
    }

    [Serializable]
    public sealed class AttemptDetails
    {
        public string source = "unity-client";
        public string summary;
        public int errorCount;
        public string[] errors = new string[0];
    }

    [Serializable]
    public sealed class AttemptResponse
    {
        public int id;
        public string profileId;
        public string actId;
        public string result;
        public AttemptDetails details;
        public string createdAt;
    }

    [Serializable]
    public sealed class HintRequest
    {
        public string profileId;
        public string actId;
        public string level = "1";
        public string trigger = "unspecified";
        public HintState state = new HintState();
    }

    [Serializable]
    public sealed class HintState
    {
        public string summary;
    }

    [Serializable]
    public sealed class HintResponse
    {
        public string hint;
        public string source;
    }

    [Serializable]
    public sealed class GeneratedResponse
    {
        public string text;
        public string source;
    }
}
