using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class RockObstacle : Obstacle
{
    public bool isOriginalRock = false;
    private int _clustersCount = 2;
    
    public void Init(Vector2 initialVelocity, SizeType sizeType)
    {
        base.Init(initialVelocity, sizeType);
        SetClustersCount();
        gameObject.tag = "rock";
    }
    
    public void Start()
    {
        base.Start();
    }
    
    void FixedUpdate()
    {
        if (UiManager.instance && UiManager.instance.isGamePaused())
        {
            return;
        }
        
        if (!isPrefab)
        {
            transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
            TeleportToScreenBorder();   
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet") || 
            other.gameObject.CompareTag("alien"))
        {
            if (other.gameObject.name.Contains("player"))
            {
                if (other.gameObject.CompareTag("bullet"))
                {
                    UiManager.instance.IncreaseScore(UiManager.instance.scoreRockWorth * ((int)_sizeType + 1));   
                }
            }
            Split();
        } else if (other.gameObject.CompareTag("player") && 
                   !other.gameObject.GetComponent<PlayerController>().IsDead() &&
                   !other.gameObject.GetComponent<PlayerController>().IsInvincible())
        {
            other.gameObject.GetComponent<PlayerController>().Die();
            Split();
        }
    }

    private void Split()
    {
        ObstaclesSpawner spawner = transform.parent.GetComponent<ObstaclesSpawner>();
        if (_clustersCount > 0)
        {
            for (int i = 0; i <  _clustersCount; i++)
            {
                double facingAngle =((Random.Range(-60, 60) + transform.rotation.eulerAngles.y) * Math.PI) / 180;
                Vector2 facingVector = new Vector2(
                    (float)Math.Cos(facingAngle),
                    -(float)Math.Sin(facingAngle));
                float speedMultiplier = Random.Range(1.1f, 1.8f);
                
                Vector2 childVelocity = facingVector * _velocity.magnitude * speedMultiplier;
                SizeType childSizeType = _sizeType - 1 >= 0 ? _sizeType - 1 : SizeType.Small;
                
                spawner.InstantiateRock(childVelocity, childSizeType, this, false);
            }
        }

        if (isOriginalRock)
        {
            spawner.DespawnRock();
        }
        Destroy(gameObject);
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
