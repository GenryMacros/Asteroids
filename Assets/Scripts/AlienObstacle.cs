using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class AlienObstacle : Obstacle
{
    public float projectileSpeed = 40.0f;
    public BulletObstacle bulletPrefab;
    public float fireCooldown = 1.0f;
    public float changeDirectionCooldown = 0.5f;
    public GameObject bulletSpawnPointPivot;
    public GameObject bulletSpawnPoint;
    public Vector2 initialDirection;
    
    private float _tilNextFire = 0.0f;
    private float _tilNextDirChange = 0.0f;
    
    public void Init(Vector2 initialVelocity, SizeType sizeType)
    {
        base.Init(initialVelocity, sizeType);
        gameObject.tag = "alien";
    }
    
    void Start()
    {
        base.Start();
        if (isPrefab)
        {
            gameObject.tag = "prefab";
        }
        else
        {
            gameObject.tag = "bullet";
        }
    }
    
    protected override void ConfigureScale()
    {
        switch (_sizeType)
        {
            case SizeType.Big:
                gameObject.transform.localScale = new Vector3(3, 3, 3);
                break;
            case SizeType.Medium:
                gameObject.transform.localScale = new Vector3(2, 2, 2);
                break;
            case SizeType.Small:
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }
    
    void FixedUpdate()
    {
        if (UiManager.instance.isGamePaused())
        {
            return;
        }
        
        if (!isPrefab)
        {
            transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
            TeleportToScreenBorder();   
            if (_tilNextFire < fireCooldown)
            {
                _tilNextFire += Time.deltaTime;
            }
            
            if (_tilNextDirChange < changeDirectionCooldown)
            {
                _tilNextDirChange += Time.deltaTime;
            }
            
            if (_tilNextFire >= fireCooldown)
            {
                Fire();
                _tilNextFire = 0;
            }
            
            if (_tilNextDirChange >= changeDirectionCooldown && IsAnyRendererVisible())
            {
                int randomVal = Random.Range(0, 100);
                if (randomVal > 70)
                {
                    ChangeDirection();
                }
                else
                {
                    _velocity = initialDirection * speed;
                }
                _tilNextDirChange = 0;
            }
        }
    }

    void ChangeDirection()
    {
        Vector2 dirChange = new Vector2((float)Math.PI / 4, (float)Math.PI / 4);
        int randomVal = Random.Range(0, 100);
        if (randomVal > 50)
        {
            dirChange.x *= -1;
        }
        else
        {
            dirChange.y *= -1;
        }

        _velocity = (initialDirection + dirChange).normalized * speed;
    }
    
    private void Fire()
    {
        BulletObstacle newBullet = Instantiate(bulletPrefab, transform.parent);

        float randomRotation = Random.Range(0, 360);
        bulletSpawnPointPivot.transform.eulerAngles = new Vector3(0.0f, randomRotation, 0.0f);
        
        double rotationY = (bulletSpawnPointPivot.transform.rotation.eulerAngles.y * Math.PI) / 180;
        Vector2 rotationDir = new Vector2(
            (float)Math.Cos(rotationY),
            -(float)Math.Sin(rotationY));
        Vector2 bulletVelocity = rotationDir * projectileSpeed;
        
        newBullet.Init(bulletVelocity, _sizeType);
        newBullet.isPrefab = false;
        newBullet.name += " alien ";
        newBullet.transform.position = bulletSpawnPoint.transform.position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        ObstaclesSpawner spawner = transform.parent.GetComponent<ObstaclesSpawner>();
        if (other.gameObject.CompareTag("rock"))
        {
            spawner.DespawnAlien();
            Destroy(gameObject);
        } else if (other.gameObject.CompareTag("player") && 
                   !other.gameObject.GetComponent<PlayerController>().IsDead() &&
                   !other.gameObject.GetComponent<PlayerController>().IsInvincible())
        {
            spawner.DespawnAlien();
            other.gameObject.GetComponent<PlayerController>().Die();
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("bullet"))
        {
            if (!other.gameObject.name.Contains("alien"))
            {
                spawner.DespawnAlien();
                UiManager.instance.IncreaseScore(UiManager.instance.scoreAlienWorth);
                Destroy(gameObject);
            }
        }
    }
    
    protected override void Disappear()
    {
        ObstaclesSpawner spawner = transform.parent.GetComponent<ObstaclesSpawner>();
        spawner.DespawnAlien();
        Destroy(gameObject);
    }
}
