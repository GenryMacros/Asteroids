using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RockSizeType
{
    Big = 0,
    Medium = 0,
    Small = 0
}


public class RockObstacle : Obstacle
{
    private RockSizeType _sizeType;
    private int _clustersCount = 0;

    public RockObstacle(Vector2 initialVelocity, RockSizeType sizeType) : base(initialVelocity)
    {
        _sizeType = sizeType;
        SetClustersCount();
    }
    
    void Start()
    {
        
    }
    
    void FixedUpdate()
    {
        transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
        CheckIfOutOfBound();
    }

    private void CheckIfOutOfBound()
    {
        Vector3 currentPosition = transform.position;
        Vector2 screenCoordinates = cam.WorldToScreenPoint(currentPosition);

        if (screenCoordinates.x > Screen.width + Screen.width * 0.1)
        {
            Destroy(this.gameObject);
        } else if (screenCoordinates.x < 0 - Screen.width * 0.1)
        {
            Destroy(this.gameObject);
        }

        if (screenCoordinates.y > Screen.height + Screen.height * 0.1)
        {
            Destroy(this.gameObject);
        } else if (screenCoordinates.y < 0 - Screen.height * 0.1)
        {
            Destroy(this.gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.GetType() == typeof(BulletObstacle) || other.GetType() == typeof(AlienObstacle))
        {
            
        }
    }

    private void Split()
    {
        if (_clustersCount != 0)
        {
            
        }
        Destroy(this.gameObject);
    }
    
    private void SetClustersCount()
    {
        if (_sizeType == RockSizeType.Big || _sizeType == RockSizeType.Medium)
        {
            _clustersCount = 2;
        } else if (_sizeType == RockSizeType.Small)
        {
            _clustersCount = 0;
        }
    }
}
