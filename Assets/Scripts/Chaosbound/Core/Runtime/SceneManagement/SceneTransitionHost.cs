
using System.Collections;
using UnityEngine;

namespace Chaosbound.Core.Runtime.SceneManagement
{
    /// <summary>
    /// Runs scene transition routines on a persistent GameObject.
    /// It does not interact with SceneManager.
    /// </summary>
    public sealed class SceneTransitionHost : MonoBehaviour
    {
        public Coroutine Run(IEnumerator routine)
        {
            if (routine == null)
            {
                Debug.LogError(
                    "[SceneTransitionHost] Routine is null.",
                    this);

                return null;
            }

            return StartCoroutine(routine);
        }
    }
}
