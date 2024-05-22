using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclesSpawner : MonoBehaviour
{
    public Camera cam;
    public RockObstacle rockPrefab;
    
    public float spawnTimeout = 1.0f;
    public float currentTime = 0.0f;
    public int maxRocksOnScreen = 5;
    public int maxAliensOnScreen = 1;
    public float maxRocksSpeed = 1;
    public float minRocksSpeed = 0.5f;
    
    private int currentRocksOnScreen = 0;
    private int currentAliensOnScreen = 0;
    
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= spawnTimeout)
        {
            Spawn();
            currentTime = 0;
        }
    }

    private void Spawn()
    {
        if (currentRocksOnScreen < maxRocksOnScreen)
        {
            int rockType = GetRandomWeightedIndex(new float[]{0.1f, 0.3f, 0.6f});
            
            float rockY = Random.Range(0, Screen.height);
            float rockX = Random.Range(0, Screen.width);

            Vector2 rockPosition = new Vector2(rockX, rockY);
            float rockRotation = Random.Range(0.0f, 360.0f);
            double rockRotationRadians = (rockRotation * Math.PI) / 180;
            
            Vector2 direction = new Vector2(
                (float)Math.Cos(rockRotationRadians),
                -(float)Math.Sin(rockRotationRadians)).normalized;
            
            
            rockPosition += -direction * (new Vector2(Screen.width, Screen.height));
            
            Vector3 velocity = direction * Random.Range(minRocksSpeed, maxRocksSpeed);
            
            RockObstacle rock = SpawnRock(velocity, (SizeType)rockType, rockPrefab);
            rock.transform.eulerAngles = new Vector3(0.0f, rockRotation, 0.0f);
            rock.transform.position = cam.ScreenToWorldPoint(rockPosition);
            rock.transform.position = new Vector3(rock.transform.position.x, 0, rock.transform.position.z);
        }
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
    
    public RockObstacle SpawnRock(Vector2 initialVelocity, SizeType sizeType, RockObstacle prefab)
    {
        RockObstacle childRock = Instantiate(prefab, transform, true);
        childRock.transform.position = prefab.transform.position;
        childRock.Init(initialVelocity, sizeType);
        currentRocksOnScreen += 1;

        return childRock;
    }
    
    public void DespawnRock()
    {
        currentRocksOnScreen -= 1;
    }
}
