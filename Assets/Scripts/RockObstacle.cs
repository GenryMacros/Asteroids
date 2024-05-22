using System;
using Siccity.GLTFUtility;
using UnityEngine;
using Random = UnityEngine.Random;


public class RockObstacle : Obstacle
{
    private int _clustersCount = 2;
    private bool isNotOnScreen = true;
    
    public void Init(Vector2 initialVelocity, SizeType sizeType)
    {
        base.Init(initialVelocity, sizeType);
        SetClustersCount();
        ConfigureScale();
        gameObject.tag = "rock";
    }
    
    void Start()
    {
        if (!visuals)
        {
            visuals = Importer.LoadFromFile(Application.dataPath + "/Visuals/rock_placeholder.glb");
            visuals.transform.position = transform.position;
            visuals.transform.parent = transform;
        }
    }

    private void ConfigureScale()
    {
        switch (_sizeType)
        {
            case SizeType.Big:
                transform.localScale = new Vector3(10, 10, 10);
                break;
            case SizeType.Medium:
                transform.localScale = new Vector3(5, 5, 5);
                break;
            case SizeType.Small:
                transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }
    
    void FixedUpdate()
    {
        transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
        bool isOutOfBound = IsOutOfBound();
        if (!isOutOfBound)
        {
            isNotOnScreen = false;
        } else if (isOutOfBound && !isNotOnScreen)
        {
            Destroy(this.gameObject);
        }
    }

    private bool IsOutOfBound()
    {
        Vector3 currentPosition = transform.position;
        Vector2 screenCoordinates = cam.WorldToScreenPoint(currentPosition);

        if (screenCoordinates.x > Screen.width + Screen.width * 0.1)
        {
            return true;
        } else if (screenCoordinates.x < 0 - Screen.width * 0.1)
        {
            return true;
        }

        if (screenCoordinates.y > Screen.height + Screen.height * 0.1)
        {
            return true;
        } else if (screenCoordinates.y < 0 - Screen.height * 0.1)
        {
            return true;
        }

        return false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet") || 
            other.gameObject.CompareTag("alien") || 
            other.gameObject.CompareTag("player"))
        {
            Split();
        }
    }

    private void Split()
    {
        if (_clustersCount > 0)
        {
            ObstaclesSpawner spawner = transform.parent.GetComponent<ObstaclesSpawner>();

            for (int i = 0; i <  _clustersCount; i++)
            {
                double facingAngle =((Random.Range(0, 40) + transform.rotation.eulerAngles.y) * Math.PI) / 180;
                Vector2 facingVector = new Vector2(
                    (float)Math.Cos(facingAngle),
                    -(float)Math.Sin(facingAngle));
                float speedMultiplier = Random.Range(1.5f, 2.5f);
                
                Vector2 childVelocity = facingVector * _velocity.magnitude * speedMultiplier;
                SizeType childSizeType = _sizeType - 1 >= 0 ? _sizeType - 1 : SizeType.Small;
                
                spawner.SpawnRock(childVelocity, childSizeType, this);
            }
            spawner.DespawnRock();
        }
        Destroy(this.gameObject);
    }
    
    private void SetClustersCount()
    {
        if (_sizeType == SizeType.Big || _sizeType == SizeType.Medium)
        {
            _clustersCount = 2;
        } else if (_sizeType == SizeType.Small)
        {
            _clustersCount = 0;
        }
    }
    
}
