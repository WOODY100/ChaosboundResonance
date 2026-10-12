using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    public static class ExpeditionProgressDataValidator
    {
        public static bool IsValid(
            ExpeditionProgressData data)
        {
            if (data == null)
                return false;

            if (data.CompletedExpeditionIds == null)
                return false;

            HashSet<string> uniqueIds =
                new HashSet<string>(
                    StringComparer.Ordinal);

            foreach (string expeditionId
                in data.CompletedExpeditionIds)
            {
                if (string.IsNullOrWhiteSpace(
                    expeditionId))
                {
                    return false;
                }

                if (!uniqueIds.Add(expeditionId))
                    return false;
            }

            return true;
        }
    }
}
