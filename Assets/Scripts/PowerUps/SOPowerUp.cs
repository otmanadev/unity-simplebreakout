using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "Custom Objects/Power Up")]
public class SOPowerUp : ScriptableObject
{
    
    [Header("Type")]
    [SerializeField] private EPowerUp type;
    public EPowerUp Type => type;
    
    [Header("Sprite")]
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
    
    [Header("Brick Material")]
    [SerializeField] private Material brickMaterial;
    public Material BrickMaterial => brickMaterial;

}
