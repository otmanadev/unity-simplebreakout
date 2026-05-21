using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class PlatformsManager : MonoBehaviour
{
    
    public static PlatformsManager Instance;

    private int _platformsCount;
    private readonly List<Platform> _allPlatforms = new();
    private readonly List<Platform> _spawnedPlatforms = new();

    [Header("Controls")] 
    [SerializeField, UnityEngine.Range(.5f, 2f)] private float sensitivityHorizontalPlatformVelocity = 1.5f;

    [Header("Platform movement fade properties")] 
    [SerializeField, Min(.0f)] private float movementFadeDuration;
    [UnityEngine.Range(0f, 1f)] private float _movementMultiplier = .0f;
    private float _currentMovementFadeDuration = .0f;
    private bool _fadeInProgress = false;
    
    [Header("Metadatas : Platform Sizes")] 
    [SerializeField] private SOPlatformSize extraLargePlatform;
    [SerializeField] private SOPlatformSize largePlatform;
    [SerializeField] private SOPlatformSize mediumPlatform;
    [SerializeField] private SOPlatformSize smallPlatform;
    [SerializeField] private SOPlatformSize extraSmallPlatform;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[<color=orange>PlatformsManager / {name}</color>] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Platform sizes
        Assert.IsNotNull(extraLargePlatform);
        Assert.IsTrue(extraLargePlatform.PlatformSize.Equals(EPlatformSize.ExtraLarge));
        
        Assert.IsNotNull(largePlatform);
        Assert.IsTrue(largePlatform.PlatformSize.Equals(EPlatformSize.Large));
        
        Assert.IsNotNull(mediumPlatform);
        Assert.IsTrue(mediumPlatform.PlatformSize.Equals(EPlatformSize.Medium));
        
        Assert.IsNotNull(smallPlatform);
        Assert.IsTrue(smallPlatform.PlatformSize.Equals(EPlatformSize.Small));
        
        Assert.IsNotNull(extraSmallPlatform);
        Assert.IsTrue(extraSmallPlatform.PlatformSize.Equals(EPlatformSize.ExtraSmall));
        
        // Platforms
        _platformsCount = GameObject.FindGameObjectsWithTag("Player")
            .Where(o => o.GetComponent<Platform>() != null)
            .Count();
    }

    private void Update()
    {
        UpdateMovementMultiplier();
        UpdateMousePositions();
    }

    private void UpdateMousePositions()
    {
        float delta = _movementMultiplier * sensitivityHorizontalPlatformVelocity * Input.GetAxis("Mouse X");
        foreach (Platform platform in _allPlatforms)
        {
            platform.InputHorizontalDirection = delta;
        }
    }

    private void UpdateMovementMultiplier()
    {
        if (!_fadeInProgress) return;

        _currentMovementFadeDuration += Time.deltaTime;
        
        if (_currentMovementFadeDuration >= movementFadeDuration)
        {
            _currentMovementFadeDuration = .0f;
            _movementMultiplier = 1.0f;
            _fadeInProgress = false;
            return;
        }

        _movementMultiplier = _currentMovementFadeDuration / movementFadeDuration;
    }

    /// <summary>
    /// Receive notification from platform when instanciated.
    /// </summary>
    /// <param name="platform"></param>
    public void OnPlatformInitializedNotification(Platform platform)
    {
        if (_allPlatforms.Contains(platform))
        {
            Debug.LogWarning($"[<color=orange>PlatformsManager / {name}</color>] Received notification from Platform {platform.name} but was already initialized");
            return;
        }
        
        _allPlatforms.Add(platform);
        VerifyIfAllPlatformsAreInstanciatedBeforeNotifyLevelManager();
    }
    
    /// <summary>
    /// Verify if all platforms are instanciated before notify Level Manager so that the level can start.
    /// </summary>
    private void VerifyIfAllPlatformsAreInstanciatedBeforeNotifyLevelManager()
    {
        if (_allPlatforms.Count != _platformsCount)
            return;
        
        Debug.Log($"[<color=orange>PlatformsManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnPlatformsManagerSuccesfullyNotified();
    }

    /// <summary>
    /// Spawn all platforms.
    /// </summary>
    public void SpawnAllPlatforms()
    {
        foreach (Platform platform in _allPlatforms)
        {
            platform.SpawnPlatform();
        }
    }
    
    /// <summary>
    /// Receive notification from single platform when spawned.
    /// </summary>
    /// <param name="platform"></param>
    public void OnPlatformSpawnedNotification(Platform platform)
    {
        if (_spawnedPlatforms.Contains(platform))
        {
            Debug.LogWarning($"[<color=orange>PlatformsManager / {name}</color>] Received notification from Platform {platform.name} but was already spawned");
            return;
        }
        
        _spawnedPlatforms.Add(platform);
        VerifyIfAllPlatformsAreSpawnedBeforeNotifyLevelManager();
    }

    /// <summary>
    /// Verify if all platforms are spawned before notify Level Manager so that the level can start.
    /// </summary>
    private void VerifyIfAllPlatformsAreSpawnedBeforeNotifyLevelManager()
    {
        if (_spawnedPlatforms.Count != _platformsCount)
            return;
        
        Debug.Log($"[<color=orange>PlatformsManager / {name}</color>] Notify Level Manager");
        LevelManager.Instance.OnPlatformsManagerSuccesfullyNotified();
    }

    /// <summary>
    /// Start move platform.
    /// </summary>
    public void StartMovePlatforms()
    {
        _fadeInProgress = true;
    }
    
    /// <summary>
    /// Enable given power up for every platforms.
    /// </summary>
    /// <param name="powerUpType"></param>
    public void ActivatePlatformPowerUp(EPowerUp powerUpType)
    {
        foreach (Platform platform in _allPlatforms)
        {
            switch (powerUpType)
            {
                case EPowerUp.PlatformBigger:
                case EPowerUp.PlatformSmaller:
                    EPlatformSize currentPlatformSizeType = platform.PlatformSize;
                    EPlatformSize newPlatformSizeType = GetNewPlatformSizePropertiesFromPowerUp(powerUpType, currentPlatformSizeType);
                    platform.SetUpNewPlatformSize(powerUpType, newPlatformSizeType);
                    break;
            }
        }
    }
    
    private EPlatformSize GetNewPlatformSizePropertiesFromPowerUp(EPowerUp powerUpType, EPlatformSize currentPlatformSizeType)
    {
        switch (currentPlatformSizeType)
        {
            case EPlatformSize.ExtraLarge:
                if (powerUpType.Equals(EPowerUp.PlatformSmaller))
                    return EPlatformSize.Large;
                break;
            case EPlatformSize.Large:
                if (powerUpType.Equals(EPowerUp.PlatformBigger))
                    return EPlatformSize.ExtraLarge;
                if (powerUpType.Equals(EPowerUp.PlatformSmaller))
                    return EPlatformSize.Medium;
                break;
            case EPlatformSize.Medium:
                if (powerUpType.Equals(EPowerUp.PlatformBigger))
                    return EPlatformSize.Large;
                if (powerUpType.Equals(EPowerUp.PlatformSmaller))
                    return EPlatformSize.Small;
                break;
            case EPlatformSize.Small:
                if (powerUpType.Equals(EPowerUp.PlatformBigger))
                    return EPlatformSize.Medium;
                if (powerUpType.Equals(EPowerUp.PlatformSmaller))
                    return EPlatformSize.ExtraSmall;
                break;
            case EPlatformSize.ExtraSmall:
                if (powerUpType.Equals(EPowerUp.PlatformBigger))
                    return EPlatformSize.Large;
                break;
        }

        return currentPlatformSizeType;
    }
    
    /// <summary>
    /// Return right platform size properties depending on its type.
    /// </summary>
    /// <param name="platformSizeType"></param>
    /// <returns></returns>
    public SOPlatformSize GetPlatformSizeByItsType(EPlatformSize platformSizeType)
    {
        switch (platformSizeType)
        {
            case EPlatformSize.ExtraLarge:
                return extraLargePlatform;
            case EPlatformSize.Large:
                return largePlatform;
            case EPlatformSize.Medium:
                return mediumPlatform;
            case EPlatformSize.Small:
                return smallPlatform;
            case EPlatformSize.ExtraSmall:
                return extraSmallPlatform;
            default:
                return mediumPlatform;
        }
    }
    
}
