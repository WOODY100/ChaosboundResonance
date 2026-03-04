using UnityEngine;

public class WallBaseRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] variants;

    void Awake()
    {
        if (variants == null || variants.Length == 0)
            return;

        int index = Random.Range(0, variants.Length);

        for (int i = 0; i < variants.Length; i++)
        {
            variants[i].SetActive(i == index);
        }
    }
}