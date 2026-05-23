using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StaticCollider : MonoBehaviour
{
    
    private void Awake()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        
        if (collider2D.isTrigger)
        {
            collider2D.isTrigger = false;
        }
    }
}
