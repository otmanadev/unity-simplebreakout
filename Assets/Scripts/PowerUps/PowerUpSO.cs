using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpSO", menuName = "Custom Objects/Power Up")]
public class PowerUpSO : ScriptableObject
{

    [SerializeField] private GameObject powerUpPrefab;
    public GameObject PowerUpPrefab => powerUpPrefab;
    
    [SerializeField] private Material material;
    public Material Material => material;

}
