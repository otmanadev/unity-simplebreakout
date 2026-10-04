using UnityEngine;

public static class LayerUtils
{

    /// <summary>
    /// Ajoute un LayerMask dans une Base LayerMask.
    /// </summary>
    /// <param name="collider2D"></param>
    /// <param name="layerToAdd"></param>
    public static void AddExcludeLayerToCollider(Collider2D collider2D, LayerMask layerToAdd)
    {
        collider2D.excludeLayers |= layerToAdd;
    }
    
    /// <summary>
    /// Retire un LayerMask dans une Base LayerMask.
    /// </summary>
    /// <param name="collider2D"></param>
    /// <param name="layerToRemove"></param>
    public static void RemoveExcludeLayerToCollider(Collider2D collider2D, LayerMask layerToRemove)
    {
        collider2D.excludeLayers &= ~layerToRemove;
    }
    
}