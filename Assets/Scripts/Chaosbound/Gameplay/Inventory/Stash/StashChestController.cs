using System.Collections;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.Stash
{
    public sealed class StashChestController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform chestLid;
        [SerializeField] private GameObject woodenBar;

        [Header("Animation")]
        [SerializeField] private float openDuration = 0.5f;
        [SerializeField] private float closeDuration = 0.4f;

        private Quaternion closedRotation;
        private Quaternion openRotation;

        private Coroutine animationRoutine;
        private bool isOpen;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            if (chestLid == null)
            {
                UnityEngine.Debug.LogError(
                    "[StashChestController] Chest Lid reference is missing.",
                    this);
                return;
            }

            closedRotation = chestLid.localRotation;

            Vector3 openEuler = closedRotation.eulerAngles;
            openEuler.x = -180f;

            openRotation = Quaternion.Euler(openEuler);

            if (woodenBar == null)
            {
                UnityEngine.Debug.LogError(
                    "[StashChestController] Wooden Bar reference is missing.",
                    this);
            }
        }

        public void Open()
        {
            if (isOpen)
                return;

            StartAnimation(true);
        }

        public void Close()
        {
            StartAnimation(false);
        }

        public void Toggle()
        {
            if (isOpen)
                Close();
            else
                Open();
        }

        private void StartAnimation(bool open)
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
            }

            animationRoutine = StartCoroutine(
                AnimateChest(open));
        }

        private IEnumerator AnimateChest(bool open)
        {
            if (open)
            {
                if (woodenBar != null)
                    woodenBar.SetActive(false);
            }

            Quaternion startRotation = chestLid.localRotation;
            Quaternion targetRotation =
                open ? openRotation : closedRotation;

            float duration =
                open ? openDuration : closeDuration;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                // SmoothStep
                t = t * t * (3f - 2f * t);

                chestLid.localRotation =
                    Quaternion.Slerp(
                        startRotation,
                        targetRotation,
                        t);

                yield return null;
            }

            chestLid.localRotation = targetRotation;

            isOpen = open;

            if (!open)
            {
                if (woodenBar != null)
                    woodenBar.SetActive(true);
            }

            animationRoutine = null;
        }
    }
}