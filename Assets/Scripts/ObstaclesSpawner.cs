using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ObstaclesSpawner : MonoBehaviour
{
    public static ObstaclesSpawner instance;
    
    public Camera cam;
    public RockObstacle rockPrefab;
    public AlienObstacle alienPrefab;

    public float timeUntilMaxDifficulty;
    public int initialRocks;
    public float alienSpawnDelay;
    private float spawnTimeout = 3.0f;
    public float currentTime = 0.0f;
    public int maxRocksOnScreen = 5;
    public int maxAliensOnScreen = 1;
    public float maxRocksSpeed = 1;
    public float minRocksSpeed = 0.5f;
    public float maxAlienSpeed = 2.0f;
    public float minAlienSpeed = 2.0f;
    
    private int currentRocksOnScreen;
    private int currentAliensOnScreen;
    private Vector2 screenSize;
    private float timeFromStart;
    private List<Vector3> rockSpawningPositions = new List<Vector3>();
    
    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        screenSize = new Vector2(Screen.width / 3.0f, Screen.height / 3.0f);
        timeFromStart = 0;
        
        foreach (Transform child in transform.GetComponentsInChildren<Transform>()) {
            if (child.CompareTag("Respawn"))
            {
                rockSpawningPositions.Add(child.position);
            }
        }
        
        for (int i = 0; i < initialRocks; i++)
        {
            SpawnRock(rockSpawningPositions[Math.Min(i, rockSpawningPositions.Count - 1)]);
        }
    }
    
    void FixedUpdate()
    {
        timeFromStart += Time.deltaTime;
        if (UiManager.instance && UiManager.instance.isGamePaused())
        {
            return;
        }
        
        currentTime += Time.deltaTime;
        if (currentTime >= spawnTimeout)
        {
            Spawn();
            currentTime = 0;
        }
        
        float t = Math.Min(timeFromStart / timeUntilMaxDifficulty, 1.0f);
        spawnTimeout = spawnTimeout * (1.0f - t) + t;
        
        maxRocksOnScreen = (int)(maxRocksOnScreen * (1.0f - t) + (maxRocksOnScreen * 3) * t);
        maxAliensOnScreen = (int)(maxAliensOnScreen * (1.0f - t) + (maxAliensOnScreen * 2) * t);
    }

    private void Spawn()
    {
        int alienSpawnRandomValue = Random.Range(0, 100);
        
        if (currentRocksOnScreen < maxRocksOnScreen)
        {
            SpawnRock(Vector3.zero);
        }

        if (alienSpawnRandomValue > 70 && currentAliensOnScreen < maxAliensOnScreen && timeFromStart >= alienSpawnDelay)
        {
            SpawnAlien();
        }
    }

    private void SpawnRock(Vector3 spawnPosition)
    {
        int rockType = GetRandomWeightedIndex(new float[]{0.2f, 0.4f, 0.4f});
        
        float rotation = Random.Range(0.0f, 360.0f);
        double rotationRadians = (rotation * Math.PI) / 180;
            
        Vector2 direction = new Vector2(
            (float)Math.Cos(rotationRadians),
            -(float)Math.Sin(rotationRadians)).normalized;
        
        float speed = Random.Range(minRocksSpeed, maxRocksSpeed);
        Vector3 velocity = direction * speed;
        RockObstacle rock = InstantiateRock(velocity, (SizeType)rockType, rockPrefab, true);
        if (spawnPosition == Vector3.zero)
        {
            float rockY = Random.Range(0, screenSize.y);
            float rockX = Random.Range(0, screenSize.x);

            Vector2 position = new Vector2(rockX, rockY);
            position = cam.ScreenToWorldPoint(new Vector3(position.x, 0, position.y));
            
            position = cam.WorldToViewportPoint(new Vector3(position.x, 0, position.y));
            position += -direction * (Vector2.one - position);
            position = MakePositionOutOfBoudns(position);
            rock.transform.position = cam.ViewportToWorldPoint(position);
            rock.transform.position = new Vector3(rock.transform.position.x, 0, rock.transform.position.z);
        }
        else
        {
            rock.transform.position = new Vector3(spawnPosition.x, 0, spawnPosition.z);
        }
        
        rock.transform.eulerAngles = new Vector3(0.0f, rotation, 0.0f);
        rock.speed = speed;
    }
    
    private void SpawnAlien()
    {
        int alienType = GetRandomWeightedIndex(new float[]{0.5f, 0.5f});
            
        float alienY = Random.Range(0, screenSize.y);
        float alienX = Random.Range(0, screenSize.x);

        Vector2 position = new Vector2(alienX, alienY);
        position = cam.ScreenToWorldPoint(new Vector3(position.x, 0, position.y));
        float rotation = Random.Range(0.0f, 360.0f);
        double rotationRadians = (rotation * Math.PI) / 180;
            
        Vector2 direction = new Vector2(
            (float)Math.Cos(rotationRadians),
            -(float)Math.Sin(rotationRadians)).normalized;
            
            
        position = cam.WorldToViewportPoint(new Vector3(position.x, 0, position.y));
        position += -direction * (Vector2.one - position);
        position = MakePositionOutOfBoudns(position);
        
        float speed = Random.Range(minAlienSpeed, maxAlienSpeed);
        Vector3 velocity = direction * speed;
            
        AlienObstacle alien = InstantiateAlien(velocity, (SizeType)alienType, alienPrefab);
        
        alien.transform.position = cam.ScreenToWorldPoint(position);
        alien.transform.position = new Vector3(alien.transform.position.x, 0, alien.transform.position.z);
        alien.speed = speed;
        alien.initialDirection = direction;
    }

    private Vector2 MakePositionOutOfBoudns(Vector2 position)
    {
        if (position.x - 0.1 < 1)
        {
            position.x = 1.1f;
        } else if (position.x + 0.1 > 0)
        {
            position.x  = -0.1f;
        }

        if (position.y - 0.1 < 1)
        {
            position.y = 1.1f;
        } else if (position.y + 0.1 > 0)
        {
            position.y = -0.1f;
        }

        return position;
    }
    
    private int GetRandomWeightedIndex(float[] weights)
    {
        float weightSum = 0f;
        for (int i = 0; i < weights.Length; ++i)
        {
            weightSum += weights[i];
        }
        
        int index = weights.Length - 1;
        while (index > 0)
        {
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }
            weightSum -= weights[index];
            index -= 1;
        }
        return index;
    }
    
    public RockObstacle InstantiateRock(Vector2 initialVelocity, SizeType sizeType, RockObstacle prefab, bool isOriginal)
    {
        RockObstacle childRock = Instantiate(prefab, transform, true);
        childRock.transform.position = prefab.transform.position;
        childRock.Init(initialVelocity, sizeType);
        childRock.isPrefab = false;
        childRock.isOriginalRock = isOriginal;
        if (childRock.isOriginalRock)
        {
            currentRocksOnScreen += 1;
        }

        childRock.transform.eulerAngles = new Vector3(Random.Range(-45, 45), Random.Range(0, 180), 0);
        return childRock;
    }
    
    private AlienObstacle InstantiateAlien(Vector2 initialVelocity, SizeType sizeType, AlienObstacle prefab)
    {
        AlienObstacle childAlien = Instantiate(prefab, transform, true);
        childAlien.transform.position = prefab.transform.position;
        childAlien.Init(initialVelocity, sizeType);
        childAlien.isPrefab = false;
        currentAliensOnScreen += 1;
    
        return childAlien;
    }
    
    public void DespawnRock()
    {
        currentRocksOnScreen -= 1;
    }
    
    public void DespawnAlien()
    {
        currentAliensOnScreen -= 1;
    }

    public void Reset()
    {
        currentRocksOnScreen = 0;
        currentAliensOnScreen = 0;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>()) {
            if (!child.CompareTag("spawner") && !child.CompareTag("Respawn"))
            {
                Destroy(child.gameObject);
            }
        }
        Start();
    }
}
