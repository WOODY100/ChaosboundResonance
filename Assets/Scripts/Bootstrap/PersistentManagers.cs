using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManagers : MonoBehaviour
{
    private static PersistentManagers instance;

    [Header("Initial Scene")]
    [SerializeField] private string firstSceneToLoad = "Arena";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.LoadScene(firstSceneToLoad);
    }
}