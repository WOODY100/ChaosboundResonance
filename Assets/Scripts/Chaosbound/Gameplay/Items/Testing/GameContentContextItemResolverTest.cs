using UnityEngine;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Items.Testing
{
    public sealed class GameContentContextItemResolverTest
        : MonoBehaviour
    {
        [ContextMenu("Run Game Content Context Item Resolver Test")]
        private void RunTest()
        {
            GameContentContext context =
                GameContentContext.Current;

            if (context == null)
            {
                Debug.LogError(
                    "[Game Content Context Item Resolver Test] " +
                    "GameContentContext.Current is null.");
                return;
            }

            ItemContentResolver resolver =
                context.ItemContentResolver;

            if (resolver == null)
            {
                Debug.LogError(
                    "[Game Content Context Item Resolver Test] " +
                    "ItemContentResolver is null.");
                return;
            }

            if (context.ItemDatabase == null)
            {
                Debug.LogError(
                    "[Game Content Context Item Resolver Test] " +
                    "ItemDatabase is null.");
                return;
            }

            if (context.ExpeditionRewardItemDatabase == null)
            {
                Debug.LogError(
                    "[Game Content Context Item Resolver Test] " +
                    "ExpeditionRewardItemDatabase is null.");
                return;
            }

            Debug.Log(
                "[Game Content Context Item Resolver Test] " +
                "✓ COMPLETE — ItemContentResolver is correctly " +
                "composed and accessible through GameContentContext.");
        }
    }
}