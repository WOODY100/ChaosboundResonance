using UnityEngine;

public class FloatingPickup : MonoBehaviour 
{ 
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 90f;
    private Vector3 startPos;
    private bool isDisabled;

    void Start() 
    {
        startPos = transform.position; 
    }
    void Update()
    {
        if (isDisabled)
            return;

        transform.position = startPos + Vector3.up *
            Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void DisableFloating()
    {
        isDisabled = true;
    }
}