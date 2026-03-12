using UnityEngine;

public class RotateAndLevitateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 100f;

    [Header("Levitation Settings")]
    [SerializeField] private float floatAmplitude = 0.5f; 
    [SerializeField] private float floatFrequency = 2f;   

    private Vector3 startLocalPos;

    void Start()
    {
        //Save the initial local position
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime, Space.Self);

        float newZ = startLocalPos.z + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);
    }
}