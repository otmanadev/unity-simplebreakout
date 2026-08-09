using System;
using UnityEngine;

[Serializable]
public class MovementBonus
{

    private int _instanceIdSource;
    public int InstanceIdSource => _instanceIdSource;

    [SerializeField, Range(-100f, 100f)] private float movementSpeedPercentage;
    public float MovementSpeedPercentage => movementSpeedPercentage;
    
    /// <summary>
    /// Définit l'instance id pour identifier le GameObject source
    /// </summary>
    /// <param name="instanceIdSource"></param>
    public void InitializeInstanceId(int instanceIdSource)
    {
        _instanceIdSource = instanceIdSource;
    }

}