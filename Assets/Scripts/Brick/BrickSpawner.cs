using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    
    public static BrickSpawner Instance;
    
    [SerializeField] private GameObject brickPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[BrickSpawner / {name}] Instance is not unique : this instance will not be created");
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(brickPrefab, $"[BrickSpawner / {name}] Brick prefab is empty.");
    }

    /// <summary>
    /// Create bricks instances.
    /// </summary>
    public void SpawnBrick()
    {
        Debug.Log($"[BrickSpawner / {name}] Spawning bricks...");
        GameObject brick = Instantiate(brickPrefab, Vector3.zero, Quaternion.identity, transform);
    }
    
}
