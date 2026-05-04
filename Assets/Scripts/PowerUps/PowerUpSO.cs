using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpSO", menuName = "Custom Objects/Power Up")]
public class PowerUpSO : ScriptableObject
{

    [SerializeField] private GameObject powerUpPrefab;
    public GameObject PowerUpPrefab => powerUpPrefab;
    
    [SerializeField] private Color glowColor;
    public Color GlowColor => glowColor;
    
    [SerializeField] private float glowStrength;
    public float GlowStrength => glowStrength;

}
