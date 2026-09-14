using System;
using Chaosbound.Content.Items;
using Chaosbound.Content.Expeditions.Runtime.References;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Items.Runtime;
using Chaosbound.Gameplay.Items.World.Integration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chaosbound.Gameplay.Items.Testing
{
    public sealed class ItemWorldDropTest : MonoBehaviour
    {
        [Header("Item")]

        [SerializeField]
        private ItemDatabase itemDatabase;

        [SerializeField]
        private string itemContentId = "sword_basic";

        [Header("Input")]

        [SerializeField]
        private Key testKey = Key.K;

        private ItemResolver itemResolver;

        private void Awake()
        {
            if (itemDatabase != null)
            {
                itemResolver =
                    new ItemResolver(
                        itemDatabase);
            }
        }

        private void Update()
        {
            if (Keyboard.current == null)
                return;

            if (!Keyboard.current[testKey].wasPressedThisFrame)
                return;

            ExecuteTest();
        }

        private void ExecuteTest()
        {
            RunManager runManager =
                RunManager.Instance;

            if (runManager == null)
            {
                Debug.LogError(
                    "ItemWorldDropTest: RunManager is not available.",
                    this);

                return;
            }

            ItemWorldDropService dropService =
                runManager.ItemWorldDropService;

            if (dropService == null)
            {
                Debug.LogError(
                    "ItemWorldDropTest: ItemWorldDropService is not available.",
                    this);

                return;
            }

            if (runManager.CurrentRunConfig == null)
            {
                Debug.LogError(
                    "ItemWorldDropTest: CurrentRunConfig is not available.",
                    this);

                return;
            }

            ExpeditionRuntimeState runtimeState =
                runManager.ExpeditionRuntimeState;

            if (runtimeState == null)
            {
                Debug.LogError(
                    "ItemWorldDropTest: ExpeditionRuntimeState is not available.",
                    this);

                return;
            }

            ExpeditionSceneContext sceneContext =
                ExpeditionSceneContext.Current;

            if (sceneContext == null ||
                sceneContext.Player == null)
            {
                Debug.LogError(
                    "ItemWorldDropTest: ExpeditionSceneContext.Player is not available.",
                    this);

                return;
            }

            if (itemResolver == null)
            {
                Debug.LogError(
                    "ItemWorldDropTest: ItemDatabase is not assigned.",
                    this);

                return;
            }

            ItemBaseData itemData;

            if (!itemResolver.TryResolve(
                itemContentId,
                out itemData))
            {
                Debug.LogError(
                    $"ItemWorldDropTest: Item '{itemContentId}' could not be resolved.",
                    this);

                return;
            }

            ItemInstance item =
                new ItemInstance(
                    Guid.NewGuid().ToString("N"),
                    itemData.ContentId,
                    itemData.BaseTier);

            RuntimeReferencesConfig references =
                new RuntimeReferencesConfig(
                    sceneContext.Player.transform);

            bool success =
                dropService.TryDrop(
                    item,
                    runManager.CurrentRunConfig,
                    references,
                    runtimeState);

            Debug.Log(
                success
                    ? $"ItemWorldDropTest: WorldItem created successfully for '{itemData.DisplayName}'."
                    : $"ItemWorldDropTest: WorldItem creation FAILED for '{itemData.DisplayName}'.",
                this);
        }
    }
}