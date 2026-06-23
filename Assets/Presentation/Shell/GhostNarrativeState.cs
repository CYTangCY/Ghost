using System;
using System.Collections.Generic;

namespace Ghost.Presentation.Shell
{
    public static class GhostNarrativeState
    {
        public const string Act1Id = "act1";
        public const string Act2Id = "act2";
        public const string Act3Id = "act3";
        public const string DefaultPlayerName = "Junior";

        private static readonly HashSet<string> CompletedActIds = new HashSet<string>(StringComparer.Ordinal);
        private static string playerName = string.Empty;
        private static string pendingDebriefActId;

        public static string PlayerName => string.IsNullOrWhiteSpace(playerName)
            ? DefaultPlayerName
            : playerName;

        public static bool HasPlayerName => !string.IsNullOrWhiteSpace(playerName);

        public static string PendingDebriefActId => pendingDebriefActId;

        public static void SetPlayerName(string value)
        {
            playerName = string.IsNullOrWhiteSpace(value)
                ? DefaultPlayerName
                : value.Trim();
        }

        public static bool IsActCompleted(string actId)
        {
            return !string.IsNullOrWhiteSpace(actId) && CompletedActIds.Contains(actId);
        }

        public static void MarkActCompleted(string actId)
        {
            if (string.IsNullOrWhiteSpace(actId))
            {
                return;
            }

            CompletedActIds.Add(actId);
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
    }
}
