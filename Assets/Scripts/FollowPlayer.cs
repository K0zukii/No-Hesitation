using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Vector3 offset;
    
    void Start()
    {
        // Use the difference between the player's position and the camera position to create the ideal offset
        offset = transform.position - player.transform.position;
    }

    //Constantly update the camera position to follow the player using the offset + updating with LateUpdate to avoid jitter
    void LateUpdate()
    {
    if ( player != null )
        {
 	        transform.position = player.transform.position + offset;
        }
    }
}
