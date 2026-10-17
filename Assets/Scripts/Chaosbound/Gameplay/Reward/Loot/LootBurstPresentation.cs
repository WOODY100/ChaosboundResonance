using System.Collections;
using UnityEngine;

public sealed class LootBurstPresentation : MonoBehaviour
{
    [Header("Burst")]
    [SerializeField, Min(0f)]
    private float burstDuration = 0.65f;

    [SerializeField, Min(0f)]
    private float horizontalSpread = 1.1f;

    [SerializeField, Min(0f)]
    private float upwardHeight = 1.25f;

    [SerializeField, Min(0f)]
    private float landingHeight = 0.05f;

    [SerializeField]
    private AnimationCurve arcCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.35f, 1f),
            new Keyframe(1f, 0f));

    public void Play(GameObject[] objects, Vector3 origin)
    {
        if (objects == null || objects.Length == 0)
            return;

        int validCount = 0;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                validCount++;
        }

        if (validCount == 0)
            return;

        int index = 0;

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = objects[i];

            if (obj == null)
                continue;

            Vector2 direction2D =
                Random.insideUnitCircle;

            if (direction2D.sqrMagnitude < 0.01f)
                direction2D = Vector2.right;

            direction2D.Normalize();

            Vector3 direction =
                new Vector3(
                    direction2D.x,
                    0f,
                    direction2D.y);

            float spread =
                Random.Range(
                    horizontalSpread * 0.5f,
                    horizontalSpread);

            Vector3 target =
                origin + direction * spread;

            target.y = origin.y + landingHeight;

            AutoPickupBehaviour pickup =
                obj.GetComponent<AutoPickupBehaviour>();

            if (pickup != null)
                pickup.SetPickupBlocked(true);

            StartCoroutine(
                AnimateObject(
                    obj,
                    origin,
                    target,
                    index,
                    pickup));

            index++;
        }
    }

    private IEnumerator AnimateObject(
        GameObject obj,
        Vector3 origin,
        Vector3 target,
        int index,
        AutoPickupBehaviour pickup)
    {
        if (obj == null)
        {
            if (pickup != null)
                pickup.SetPickupBlocked(false);

            yield break;
        }

        Vector3 start = origin;
        obj.transform.position = start;

        float duration =
            Mathf.Max(0.01f, burstDuration);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (obj == null || !obj.activeInHierarchy)
                yield break;

            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(elapsed / duration);

            Vector3 position =
                Vector3.Lerp(start, target, t);

            position.y +=
                arcCurve.Evaluate(t) * upwardHeight;

            obj.transform.position = position;

            yield return null;
        }

        if (obj != null && obj.activeInHierarchy)
        {
            obj.transform.position = target;

            if (pickup != null)
                pickup.SetPickupBlocked(false);
        }
    }
}