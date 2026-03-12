using UnityEngine;

public class RepeatWorld : MonoBehaviour
{
    [SerializeField] private float chunkLength = 164.9468f;

    // Make the background repeat indefinitely
    void Update()
    {
        if (transform.position.z <  - (chunkLength / 1.3))
        {
            Vector3 jumpPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + (chunkLength * 2));
            transform.position = jumpPosition;
        }
    }
}
