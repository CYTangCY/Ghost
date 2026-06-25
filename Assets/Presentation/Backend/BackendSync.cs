using Ghost.Presentation.Shell;
using UnityEngine;

namespace Ghost.Presentation.Backend
{
    public static class BackendSync
    {
        private static bool hasStarted;
        private static bool isApplyingBackendProgress;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoStart()
        {
            EnsureStarted();
        }

        public static void EnsureStarted()
        {
            if (hasStarted)
            {
                return;
            }

            hasStarted = true;
            GhostNarrativeState.StateChanged -= OnNarrativeStateChanged;
            GhostNarrativeState.StateChanged += OnNarrativeStateChanged;

            GhostBackendClient.EnsureProfile(OnProfileReady);
        }

        public static void PushProgress()
        {
            if (isApplyingBackendProgress)
            {
                return;
            }

            GhostBackendClient.PutProgress(CreateProgressRequest());
        }

        private static void OnProfileReady(GhostBackendResponse<ProfileResponse> response)
        {
            if (!response.Succeeded || response.Value == null || string.IsNullOrWhiteSpace(response.Value.id))
            {
                return;
            }

            GhostBackendClient.GetProgress(response.Value.id, OnProgressLoaded);
        }

        private static void OnProgressLoaded(GhostBackendResponse<ProgressResponse> response)
        {
            if (!response.Succeeded || response.Value == null)
            {
                return;
            }

            isApplyingBackendProgress = true;
            GhostNarrativeState.ApplyBackendProgress(
                response.Value.narrativeState == null ? null : response.Value.narrativeState.playerName,
                response.Value.actsCompleted,
                false);
            isApplyingBackendProgress = false;
        }

        private static void OnNarrativeStateChanged()
        {
            PushProgress();
        }

        private static ProgressRequest CreateProgressRequest()
        {
            var completedActIds = GhostNarrativeState.GetCompletedActIds();

            return new ProgressRequest
            {
                actsCompleted = completedActIds,
                levelsCompleted = CreateLevelIds(completedActIds),
                narrativeState = new NarrativeStatePayload
                {
                    playerName = GhostNarrativeState.PlayerName
                }
            };
        }

        private static string[] CreateLevelIds(string[] completedActIds)
        {
            if (completedActIds == null || completedActIds.Length == 0)
            {
                return new string[0];
            }

            var levelIds = new string[completedActIds.Length];
            for (var i = 0; i < completedActIds.Length; i++)
            {
                levelIds[i] = string.IsNullOrWhiteSpace(completedActIds[i])
                    ? string.Empty
                    : completedActIds[i] + ":1";
            }

            return levelIds;
        }
    }
}
