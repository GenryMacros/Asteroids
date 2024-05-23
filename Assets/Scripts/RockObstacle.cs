using System;
using Siccity.GLTFUtility;
using UnityEngine;
using Random = UnityEngine.Random;


public class RockObstacle : Obstacle
{
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
        if (!visuals)
        {
            visuals = Importer.LoadFromFile(Application.dataPath + "/Visuals/rock_placeholder.glb");
            visuals.transform.position = transform.position;
            visuals.transform.parent = transform;
        }
    }
    
    void FixedUpdate()
    {
        if (!isPrefab)
        {
            transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
            TeleportToScreenBorder();   
        }
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
        ObstaclesSpawner spawner = transform.parent.GetComponent<ObstaclesSpawner>();
        if (_clustersCount > 0)
        {
            for (int i = 0; i <  _clustersCount; i++)
            {
                double facingAngle =((Random.Range(-60, 60) + transform.rotation.eulerAngles.y) * Math.PI) / 180;
                Vector2 facingVector = new Vector2(
                    (float)Math.Cos(facingAngle),
                    -(float)Math.Sin(facingAngle));
                float speedMultiplier = Random.Range(1.5f, 2.5f);
                
                Vector2 childVelocity = facingVector * _velocity.magnitude * speedMultiplier;
                SizeType childSizeType = _sizeType - 1 >= 0 ? _sizeType - 1 : SizeType.Small;
                
                spawner.InstantiateRock(childVelocity, childSizeType, this);
            }
        }
        spawner.DespawnRock();
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
