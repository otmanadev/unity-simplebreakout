using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "Custom Objects/Power Up")]
public class SOPowerUp : ScriptableObject
{

    [SerializeField] private GameObject powerUpPrefab;
    public GameObject PowerUpPrefab => powerUpPrefab;

}
