using Unity.AI.Navigation;
using UnityEngine;

namespace Chaosbound.Gameplay.Navigation
{
    /// <summary>
    /// Builds and owns the navigation mesh of the current expedition.
    ///
    /// Navigation is world infrastructure.
    /// It does not know about enemies or enemy behavior.
    /// </summary>
    public sealed class ExpeditionNavigation : MonoBehaviour
    {
        [Header("NavMesh")]
        [SerializeField]
        private NavMeshSurface navMeshSurface;

        [Header("Navigation Layers")]
        [SerializeField]
        private LayerMask navigationLayers;

        [Header("Build Volume")]
        [SerializeField]
        private float buildHeight = 10f;

        /// <summary>
        /// Gets whether the expedition NavMesh has been built successfully.
        /// </summary>
        public bool IsReady
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the NavMesh surface used by this expedition.
        /// </summary>
        public NavMeshSurface Surface =>
            navMeshSurface;

        /// <summary>
        /// Builds the NavMesh using the physical bounds generated
        /// by the World system.
        /// </summary>
        public void Initialize(
            Bounds generatedWorldBounds)
        {
            if (navMeshSurface == null)
            {
                throw new System.InvalidOperationException(
                    $"{nameof(ExpeditionNavigation)} " +
                    "requires a NavMeshSurface reference.");
            }

            if (generatedWorldBounds.size.x <= 0f ||
                generatedWorldBounds.size.z <= 0f)
            {
                throw new System.InvalidOperationException(
                    "Generated world bounds are invalid.");
            }

            IsReady = false;

            ConfigureSurface(
                generatedWorldBounds);

            BuildNavMesh();

            IsReady =
                navMeshSurface.navMeshData != null;
        }

        private void ConfigureSurface(
            Bounds generatedWorldBounds)
        {
            navMeshSurface.collectObjects =
                CollectObjects.Volume;

            navMeshSurface.center =
                new Vector3(
                    generatedWorldBounds.center.x,
                    0f,
                    generatedWorldBounds.center.z);

            navMeshSurface.size =
                new Vector3(
                    generatedWorldBounds.size.x,
                    buildHeight,
                    generatedWorldBounds.size.z);

            navMeshSurface.layerMask =
                navigationLayers;

            navMeshSurface.useGeometry =
                UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        }

        private void BuildNavMesh()
        {
            navMeshSurface.RemoveData();
            navMeshSurface.BuildNavMesh();

            if (navMeshSurface.navMeshData == null)
            {
                throw new System.InvalidOperationException(
                    "Expedition NavMesh could not be built.");
            }
        }
    }
}