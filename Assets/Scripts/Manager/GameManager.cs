using System;
using Domain;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;

    [SerializeField] private TextAsset levelConfigJson;
    
    private LevelConfiguration _levelConfig;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[GameManager / {name}] Instance is not unique : this instance will not be created");
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(levelConfigJson);
    }

    private void Start()
    {
        //InstanciateLevel();
        
        BrickSpawner brickSpawner = BrickSpawner.Instance;
        if (brickSpawner != null)
        {
            //brickSpawner.SpawnBrick();
        }
    }

    /// <summary>
    /// Read level configuration file to create level.
    /// </summary>
    private void InstanciateLevel()
    {
        _levelConfig = JsonUtility.FromJson<LevelConfiguration>(levelConfigJson.text);
        Debug.Log($"[GameManager / {name}] Read level config json : {JsonUtility.ToJson(_levelConfig)}");
        Debug.Log($"[GameManager / {name}] {_levelConfig}");
        
        
    }
    
}
