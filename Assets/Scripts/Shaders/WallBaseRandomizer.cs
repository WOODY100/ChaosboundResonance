using UnityEngine;

public class WallBaseRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] variants;

    void Awake()
    {
        if (variants == null || variants.Length == 0)
            return;

        // 🔧 Asegurar que todas estén activas primero
        for (int i = 0; i < variants.Length; i++)
        {
            if (variants[i] != null)
                variants[i].SetActive(true);
        }

        // 🎲 Elegir una variante
        int index = Random.Range(0, variants.Length);

        // 🔒 Desactivar las demás
        for (int i = 0; i < variants.Length; i++)
        {
            if (variants[i] != null)
                variants[i].SetActive(i == index);
        }
    }
}