using UnityEngine;

namespace Chaosbound.Gameplay.MetaProgression.Persistent
{
    public sealed class PersistentMetaRuntime : MonoBehaviour
    {
        private PersistentMetaState state;

        public PersistentMetaState State =>
            state;

        private void Awake()
        {
            state = new PersistentMetaState();
        }
    }
}