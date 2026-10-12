using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    public sealed class ExpeditionProgressRuntime
    {
        private readonly HashSet<string> completedExpeditionIds =
            new HashSet<string>(StringComparer.Ordinal);

        public bool HasCompleted(string expeditionId)
        {
            if (string.IsNullOrWhiteSpace(expeditionId))
                return false;

            return completedExpeditionIds.Contains(
                expeditionId);
        }

        public bool TryRegisterCompletion(string expeditionId)
        {
            if (string.IsNullOrWhiteSpace(expeditionId))
                return false;

            return completedExpeditionIds.Add(
                expeditionId);
        }

        public ExpeditionProgressData ToSaveData()
        {
            ExpeditionProgressData data =
                new ExpeditionProgressData();

            foreach (string expeditionId in completedExpeditionIds)
            {
                data.CompletedExpeditionIds.Add(
                    expeditionId);
            }

            data.CompletedExpeditionIds.Sort(
                StringComparer.Ordinal);

            return data;
        }

        public bool ApplySaveData(
            ExpeditionProgressData data)
        {
            if (data == null)
                return false;

            if (!ExpeditionProgressDataValidator.IsValid(data))
                return false;

            HashSet<string> loadedIds =
                new HashSet<string>(
                    data.CompletedExpeditionIds,
                    StringComparer.Ordinal);

            completedExpeditionIds.Clear();

            foreach (string expeditionId in loadedIds)
            {
                completedExpeditionIds.Add(
                    expeditionId);
            }

            return true;
        }
    }
}