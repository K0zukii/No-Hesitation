using UnityEngine;
using System;
public class Collectible : MonoBehaviour
{
    [SerializeField] ItemData data;
    public static event Action<float> OnItemCollected;

    //Static event to inform the GameManager without creating a direct depedency
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnItemCollected?.Invoke(data.fuelValue);
            SpawnManager.Instance.ReturnObjectToPool(gameObject);
        }
    }
}
