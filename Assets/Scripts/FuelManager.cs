using UnityEngine;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
    [Header("Fuel System")]
    [SerializeField] private float currentFuel = 100f;
    [SerializeField] private float maxFuel = 100.0f;
    [SerializeField] private float burnRate = 10f;

    [SerializeField] private GameManager gameManager;

    [Header("UI Elements")]
    [SerializeField] private Image fuelBarImage;

    private bool isOutOfFuel = false;


    private void OnEnable() {
        Collectible.OnItemCollected += AddFuel;
        
    }
    private void OnDisable()
    {
        Collectible.OnItemCollected -= AddFuel;
    }

    //Constantly check if the player have fuel, if yes decrease his amount and update the UI, if not game over !
    void Update()
    {
        if (!isOutOfFuel)
        {
            if (currentFuel > 0)
            {
                currentFuel -= burnRate * Time.deltaTime;
                fuelBarImage.fillAmount = currentFuel / maxFuel;
            }
            else
            {
                isOutOfFuel = true;
                GameOver();
            }
        }
    }

    //Fonction called by the OnItemCollected event
    void AddFuel(float value)
    {
        currentFuel += value;
        if (currentFuel > maxFuel)
        {
            currentFuel = maxFuel;
        }
        
        fuelBarImage.fillAmount = currentFuel / maxFuel;
    }

    void GameOver()
    {
        gameManager.GameOver();
    }
}
