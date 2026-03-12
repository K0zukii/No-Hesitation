using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float driveSpeed = 15f;
    [SerializeField] private float turnAngle = 0.1f;
    private float xRangeLeft = -5.80f;
    private float xRangeRight = 12f;
    private float horizontalInput;
    [SerializeField] private GameManager gameManager;

    [Header("Audio SFX")]
    [SerializeField] private AudioSource crashSound;

    [Header("VFX Crash")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject firePrefab;
    
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
    //Get player input using legacy system
    horizontalInput = Input.GetAxis("Horizontal");

    //Make the car move accordingly to player input
    transform.Translate(Vector3.right * horizontalInput * driveSpeed * Time.deltaTime, Space.World);
    transform.rotation = Quaternion.Euler(0, horizontalInput * turnAngle, 0);

    //Make sure the player doesn't go out of bounds
    Vector3 clampedPosition = transform.position;
    clampedPosition.x = Mathf.Clamp(transform.position.x, xRangeLeft, xRangeRight);
    transform.position = clampedPosition;
    }

    //If the player crashes, Game Over and VFX + SFX play
    void OnCollisionEnter(Collision collision)
    {
    
    if (collision.gameObject.CompareTag("Obstacles"))
        {
            gameManager.GameOver();

            if(crashSound != null)
                {
                    crashSound.Play();
                }
            
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            StartCoroutine(CrashVFXSequence());
        }
    }

    //Spawn fire on the car after real time seconds the explosion ended because the GameOver function stopped the in game time
    System.Collections.IEnumerator CrashVFXSequence()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GameObject fire = Instantiate(firePrefab, transform.position, transform.rotation);
        fire.transform.SetParent(this.transform);
    }
}
