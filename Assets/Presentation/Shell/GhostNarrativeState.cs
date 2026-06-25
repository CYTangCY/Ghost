using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ghost.Presentation.Shell
{
    public static class GhostNarrativeState
    {
        public const string Act1Id = "act1";
        public const string Act2Id = "act2";
        public const string Act3Id = "act3";
        public const string DefaultPlayerName = "Junior";
        public const string BackendProfileIdPlayerPrefsKey = "Ghost.Backend.ProfileId";

        private static readonly HashSet<string> CompletedActIds = new HashSet<string>(StringComparer.Ordinal);
        private static string playerName = string.Empty;
        private static string pendingDebriefActId;

        public static event Action StateChanged;

        public static string PlayerName => string.IsNullOrWhiteSpace(playerName)
            ? DefaultPlayerName
            : playerName;

        public static bool HasPlayerName => !string.IsNullOrWhiteSpace(playerName);

        public static string PendingDebriefActId => pendingDebriefActId;

        public static string BackendProfileId => PlayerPrefs.GetString(BackendProfileIdPlayerPrefsKey, string.Empty);

        public static void SetPlayerName(string value)
        {
            var previousName = PlayerName;
            var previouslyHadPlayerName = HasPlayerName;
            playerName = string.IsNullOrWhiteSpace(value)
                ? DefaultPlayerName
                : value.Trim();

            if (!previouslyHadPlayerName || !string.Equals(previousName, PlayerName, StringComparison.Ordinal))
            {
                NotifyStateChanged();
            }
        }

        public static bool IsActCompleted(string actId)
        {
            return !string.IsNullOrWhiteSpace(actId) && CompletedActIds.Contains(actId);
        }

        public static string[] GetCompletedActIds()
        {
            var completedActIds = new string[CompletedActIds.Count];
            CompletedActIds.CopyTo(completedActIds);
            Array.Sort(completedActIds, StringComparer.Ordinal);
            return completedActIds;
        }

        public static void MarkActCompleted(string actId)
        {
            if (string.IsNullOrWhiteSpace(actId))
            {
                return;
            }

            if (CompletedActIds.Add(actId))
            {
                NotifyStateChanged();
            }
        }

        public static void SetBackendProfileId(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                PlayerPrefs.DeleteKey(BackendProfileIdPlayerPrefsKey);
                PlayerPrefs.Save();
                return;
            }

            PlayerPrefs.SetString(BackendProfileIdPlayerPrefsKey, profileId.Trim());
            PlayerPrefs.Save();
        }

        public static void ApplyBackendProgress(
            string backendPlayerName,
            IEnumerable<string> completedActIds,
            bool notify)
        {
            var changed = false;

            if (!HasPlayerName && !string.IsNullOrWhiteSpace(backendPlayerName))
            {
                playerName = backendPlayerName.Trim();
                changed = true;
            }

            if (completedActIds != null)
            {
                foreach (var completedActId in completedActIds)
                {
                    if (string.IsNullOrWhiteSpace(completedActId))
                    {
                        continue;
                    }

                    if (CompletedActIds.Add(completedActId))
                    {
                        changed = true;
                    }
                }
            }

            if (changed && notify)
            {
                NotifyStateChanged();
            }
        }

        public static void SetPendingDebriefAct(string actId)
        {
            if (string.IsNullOrWhiteSpace(actId))
            {
                pendingDebriefActId = null;
                return;
            }

            pendingDebriefActId = actId;
        }

        public static string ConsumePendingDebriefAct()
        {
            var actId = pendingDebriefActId;
            pendingDebriefActId = null;
            return actId;
        }

        private static void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }
}
