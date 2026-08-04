using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.BackendResponse
{
    public static class Act6BackendResponseValidator
    {
        public static Act6BackendValidationResult Validate(
            string dataSourceId,
            string actionId,
            string responseId)
        {
            var errors = new List<string>();
            var incorrectRoleIds = new List<string>();
            var firstBrokenRoleId = string.Empty;

            ValidateRole(
                Act6BackendResponseData.DataSourceRoleId,
                dataSourceId,
                errors,
                incorrectRoleIds,
                ref firstBrokenRoleId);
            ValidateRole(
                Act6BackendResponseData.ActionRoleId,
                actionId,
                errors,
                incorrectRoleIds,
                ref firstBrokenRoleId);
            ValidateRole(
                Act6BackendResponseData.ResponseRoleId,
                responseId,
                errors,
                incorrectRoleIds,
                ref firstBrokenRoleId);

            return new Act6BackendValidationResult(errors, firstBrokenRoleId, incorrectRoleIds);
        }

        private static void ValidateRole(
            string roleId,
            string actualCardId,
            ICollection<string> errors,
            ICollection<string> incorrectRoleIds,
            ref string firstBrokenRoleId)
        {
            var expectedCardId = Act6BackendResponseData.GetExpectedCardId(roleId);
            if (string.IsNullOrWhiteSpace(actualCardId))
            {
                errors.Add(Act6BackendResponseData.GetRoleLabel(roleId) + " is empty.");
                incorrectRoleIds.Add(roleId);
                SetFirstBroken(ref firstBrokenRoleId, roleId);
                return;
            }

            Act6BackendCard actualCard;
            try
            {
                actualCard = Act6BackendResponseData.GetCard(actualCardId);
            }
            catch (ArgumentException)
            {
                errors.Add(Act6BackendResponseData.GetRoleLabel(roleId) + " contains an unknown card.");
                incorrectRoleIds.Add(roleId);
                SetFirstBroken(ref firstBrokenRoleId, roleId);
                return;
            }

            if (!string.Equals(actualCard.RoleId, roleId, StringComparison.Ordinal))
            {
                errors.Add(
                    actualCard.Label + " belongs to " +
                    Act6BackendResponseData.GetRoleLabel(actualCard.RoleId) +
                    ", not " + Act6BackendResponseData.GetRoleLabel(roleId) + ".");
                incorrectRoleIds.Add(roleId);
                SetFirstBroken(ref firstBrokenRoleId, roleId);
                return;
            }

            if (!string.Equals(actualCardId, expectedCardId, StringComparison.Ordinal))
            {
                errors.Add(actualCard.FailureLine);
                incorrectRoleIds.Add(roleId);
                SetFirstBroken(ref firstBrokenRoleId, roleId);
            }
        }

        private static void SetFirstBroken(ref string currentRoleId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(currentRoleId))
            {
                currentRoleId = roleId;
            }
        }
    }
}
