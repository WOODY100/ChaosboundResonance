using UnityEngine;
using Chaosbound.Content.Items;

namespace Chaosbound.Gameplay.Items.Runtime
{
    public sealed class ItemContentResolverTest : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField]
        private ExpeditionRewardItemDatabase
            expeditionRewardItemDatabase;

        [Header("Test Content")]
        [SerializeField] private ItemBaseData normalItem;
        [SerializeField] private ItemBaseData rewardItem;

        [ContextMenu("Run Item Content Resolver Test")]
        private void RunTest()
        {
            if (itemDatabase == null)
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Missing ItemDatabase.");
                return;
            }

            if (expeditionRewardItemDatabase == null)
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Missing ExpeditionRewardItemDatabase.");
                return;
            }

            if (normalItem == null)
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Missing normal ItemBaseData.");
                return;
            }

            if (rewardItem == null)
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Missing reward ItemBaseData.");
                return;
            }

            ItemContentResolver resolver =
                new ItemContentResolver(
                    itemDatabase,
                    expeditionRewardItemDatabase);

            // -------------------------------------------------
            // Normal Item
            // -------------------------------------------------

            if (!resolver.TryResolve(
                    normalItem.ContentId,
                    out ItemBaseData resolvedNormalItem))
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    $"Could not resolve normal item " +
                    $"'{normalItem.ContentId}'.");
                return;
            }

            if (resolvedNormalItem != normalItem)
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Normal item resolved to a different " +
                    "ItemBaseData.");
                return;
            }

            // -------------------------------------------------
            // Expedition Reward Item
            // -------------------------------------------------

            if (!resolver.TryResolve(
                    rewardItem.ContentId,
                    out ItemBaseData resolvedRewardItem))
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    $"Could not resolve reward item " +
                    $"'{rewardItem.ContentId}'.");
                return;
            }

            if (resolvedRewardItem != rewardItem)
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Reward item resolved to a different " +
                    "ItemBaseData.");
                return;
            }

            // -------------------------------------------------
            // Unknown ContentId
            // -------------------------------------------------

            const string unknownContentId =
                "__chaosbound_unknown_item__";

            if (resolver.TryResolve(
                    unknownContentId,
                    out ItemBaseData unknownItem))
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Unknown ContentId was incorrectly resolved.");
                return;
            }

            if (unknownItem != null)
            {
                Debug.LogError(
                    "[Item Content Resolver Test] " +
                    "Unknown ContentId returned a non-null item.");
                return;
            }

            Debug.Log(
                "[Item Content Resolver Test] ✓ COMPLETE — " +
                "Normal, Expedition Reward, and unknown " +
                "Item ContentIds were resolved correctly.");
        }
    }
}