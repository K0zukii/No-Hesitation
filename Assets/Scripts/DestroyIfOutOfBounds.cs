using UnityEngine;

//Constantly watch the object's position and recycle it using the Object pooling system when it gets out of the player sight
public class DestroyIfOutOfBounds : MonoBehaviour
{
    private float downBound = -50.0f;

    void Update()
    {
        if (transform.position.z < downBound)
        {
            SpawnManager.Instance.ReturnObjectToPool(gameObject);
        }
    }
}
