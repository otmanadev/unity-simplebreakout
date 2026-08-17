using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Movement
{

    [SerializeField, Min(0f)] private float movementSpeed;
    public float MovementSpeed => movementSpeed;
    
    [SerializeField] private Vector2 direction;
    public Vector2 Direction => direction;
    
    [SerializeField] private List<MovementBonus> movementBonuses = new();
    
    public Vector2 GetMovementDirection => MovementSpeed * Direction * GetMovementBonusPercentage();

    public void UpdateDirectionNormalized(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    /// <summary>
    /// Ajoute un mouvement bonus cumulé.
    /// On ne peut pas ajouter plus d'un bonus provenant de la même source.
    /// </summary>
    /// <param name="bonus"></param>
    public void AddMovementBonus(MovementBonus bonus)
    {
        if (movementBonuses.Contains(bonus))
        {
            Debug.LogWarning($"{nameof(MovementBonus)} already exists!");
            return;
        }
        
        movementBonuses.Add(bonus);
    }

    /// <summary>
    /// Retire un mouvement bonus cumulé.
    /// </summary>
    /// <param name="bonus"></param>
    public void RemoveMovementBonus(MovementBonus bonus)
    {
        movementBonuses.Remove(bonus);
    }

    /// <summary>
    /// Renvoie le cumul de tous les bonus de mouvements sous un format de pourcentage et divisé par 100.
    /// Ce calcul additionne les bonus positifs et soustrait les bonus négatifs.
    /// Un cumul total à une valeur négative remonte le résultat à 0 afin d'éviter des déplacements opposés à la direction.
    /// </summary>
    /// <returns></returns>
    private float GetMovementBonusPercentage()
    {
        float totalBonus = 1.0f + GetPositiveMovementBonusPercentage() + GetNegativeMovementBonusPercentage();
        return MathF.Max(0.0f, totalBonus); 
    }

    /// <summary>
    /// Renvoie le cumul des bonus positifs de mouvements sous forme de pourcentage et divisé par 100.
    /// </summary>
    /// <returns></returns>
    private float GetPositiveMovementBonusPercentage()
    {
        return movementBonuses
            .Where(movementBonus => movementBonus.MovementSpeedPercentage > .0f)
            .Sum(movementBonus => movementBonus.MovementSpeedPercentage) / 100f;
    }
    
    /// <summary>
    /// Renvoie le cumul des bonus négatifs de mouvements sous forme de pourcentage et divisé par 100.
    /// </summary>
    /// <returns></returns>
    private float GetNegativeMovementBonusPercentage()
    {
        return movementBonuses
            .Where(movementBonus => movementBonus.MovementSpeedPercentage < .0f)
            .Sum(movementBonus => movementBonus.MovementSpeedPercentage) / 100f;
    }

}