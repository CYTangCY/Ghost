using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.BackendResponse
{
    public sealed class Act6BackendCard
    {
        public Act6BackendCard(
            string id,
            string label,
            string roleId,
            string jobLine,
            string failureLine)
        {
            Id = id ?? string.Empty;
            Label = label ?? string.Empty;
            RoleId = roleId ?? string.Empty;
            JobLine = jobLine ?? string.Empty;
            FailureLine = failureLine ?? string.Empty;
        }

        public string Id { get; }

        public string Label { get; }

        public string RoleId { get; }

        public string JobLine { get; }

        public string FailureLine { get; }
    }

    public sealed class Act6BackendPlaybackStep
    {
        public Act6BackendPlaybackStep(string roleId, string title, string line)
        {
            RoleId = roleId ?? string.Empty;
            Title = title ?? string.Empty;
            Line = line ?? string.Empty;
        }

        public string RoleId { get; }

        public string Title { get; }

        public string Line { get; }
    }

    public sealed class Act6BackendValidationResult
    {
        private readonly string[] errors;
        private readonly string[] incorrectRoleIds;

        public Act6BackendValidationResult(
            IEnumerable<string> validationErrors,
            string firstBrokenRoleId,
            IEnumerable<string> invalidRoleIds)
        {
            errors = validationErrors == null
                ? Array.Empty<string>()
                : new List<string>(validationErrors).ToArray();
            incorrectRoleIds = invalidRoleIds == null
                ? Array.Empty<string>()
                : new List<string>(invalidRoleIds).ToArray();
            FirstBrokenRoleId = firstBrokenRoleId ?? string.Empty;
        }

        public IReadOnlyList<string> Errors => errors;

        public string FirstBrokenRoleId { get; }

        public bool IsCorrect => errors.Length == 0;

        public bool IsRoleCorrect(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                return false;
            }

            for (var index = 0; index < incorrectRoleIds.Length; index++)
            {
                if (string.Equals(incorrectRoleIds[index], roleId, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
