using UnityEngine;

public class MoveBackwards : MonoBehaviour
{

    [SerializeField] float speed;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Make the object move faster accordingly to the game speed
    void Update()
    {
        if ( gameManager != null)
        {
            float currentSpeed = speed * gameManager.speedMultiplier;
            transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
        }
       
    }
}
