using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class AlienObstacle : Obstacle
{
    public float projectileSpeed = 40.0f;
    public float fireCooldown = 1.0f;
    public float changeDirectionCooldown = 0.5f;
    public GameObject bulletSpawnPointPivot;
    public GameObject bulletSpawnPoint;
    public Vector2 initialDirection;
    public AudioClip shootClip;
    public AudioClip dieClip;
    public AudioClip spawnClip;
    
    private bool isDead = false;
    private BulletObstacle bulletPrefab;
    
    public void Init(Vector2 initialVelocity, SizeType sizeType)
    {
        base.Init(initialVelocity, sizeType);
        gameObject.tag = "alien";
        audioSource.clip = spawnClip;
        audioSource.Play();
    }
    
    void Start()
    {
        base.Start();
        bulletPrefab = Resources.Load<BulletObstacle>("BulletPrefab");
        audioSource = GetComponent<AudioSource>();
        gameObject.tag = "bullet";
        InvokeRepeating(nameof(Fire), fireCooldown, fireCooldown);
        InvokeRepeating(nameof(ChangeDirection), changeDirectionCooldown, changeDirectionCooldown);
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
        if (UiManager.instance.isGamePaused() || isDead)
        {
            return;
        }
        
        transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
        TeleportToScreenBorder();   
    }

    void ChangeDirection()
    {
        int randomVal = Random.Range(0, 100);
        if (randomVal > 80)
        {
            Vector2 dirChange = new Vector2((float)Math.PI / 4, (float)Math.PI / 4);
            randomVal = Random.Range(0, 100);
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
        else
        {
            _velocity = initialDirection * speed;
        }
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
        newBullet.name += " alien ";
        newBullet.transform.position = bulletSpawnPoint.transform.position;

        if (!audioSource.isPlaying)
        {
            audioSource.clip = shootClip;
            audioSource.Play();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        audioSource.clip = dieClip;
        ObstaclesSpawner spawner = transform.parent.GetComponent<ObstaclesSpawner>();
        if (other.gameObject.CompareTag("rock"))
        {
            spawner.DespawnAlien();
            isDead = true;
            StartCoroutine(PlayAudioAndDestroy());
        } else if (other.gameObject.CompareTag("player") && 
                   !other.gameObject.GetComponent<PlayerController>().IsDead() &&
                   !other.gameObject.GetComponent<PlayerController>().IsInvincible())
        {
            spawner.DespawnAlien();
            other.gameObject.GetComponent<PlayerController>().Die();
            isDead = true;
            StartCoroutine(PlayAudioAndDestroy());
        }
        else if (other.gameObject.CompareTag("bullet"))
        {
            if (!other.gameObject.name.Contains("alien"))
            {
                spawner.DespawnAlien();
                UiManager.instance.IncreaseScore(UiManager.instance.scoreAlienWorth);
                isDead = true;
                StartCoroutine(PlayAudioAndDestroy());
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
