using System;
using System.Collections.Generic;
using Siccity.GLTFUtility;
using UnityEngine.InputSystem;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    public float maxSpeed = 10f;
    public float rotationSpeed = 100f;
    public float accelerationIncreaseRate = 2f;
    public float oppositePower = 0.01f;
    public float fireCooldown = 1.0f;
    public float projectileSpeed = 30f;
    public float deathDuration = 1.0f;
    public float invincibilityDuration = 1.0f;
    public float flickeringInterval = 0.2f;
    public Camera cam;
    public BulletObstacle bulletPrefab;
    public GameObject bulletSpawnPoint;
    public GameObject body;
    public int lives = 3;
    
    public InputAction moveAction;
    public InputAction shootAction;
    public AudioClip loopSound;
    public AudioClip shootSound;
    public AudioClip destructionSound;
    
    private Renderer[] renderers;
    
    private Vector2 _velocity = Vector2.zero;
    private Vector2 _lastMovingDirection = Vector2.zero;
    private float _rotationY; 
    private Vector2 _screenZeroCoordsInWorld;
    private Vector2 _screenMaxCoordsInWorld;
    private float _tilInvincibilityEnd = 0.0f;
    private float _tilNextFire = 0.0f;
    private float _tilNextFlicker = 0.0f;
    private float _deathTime = 0.0f;
    private bool _isDead = false;
    private bool _isInvincible = false;

    private AudioSource[] _audioSources;
    private SphereCollider _collider;
    private Rigidbody _rigidbody;
    private ParticleSystem engineParticles;
    
    private bool isWrappingX = false;
    private bool isWrappingZ = false;
    
    private void Awake()
    {
        instance = this;
    }
    
    private void OnEnable()
    {
        moveAction.Enable();
        shootAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        shootAction.Disable();
    }

    void Start()
    {
        _audioSources = GetComponentsInChildren<AudioSource>();
        _audioSources[1].clip = shootSound;
        renderers = GetComponentsInChildren<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _screenZeroCoordsInWorld = cam.ScreenToWorldPoint(Vector2.zero);
        _screenZeroCoordsInWorld.y *= -1;
        _screenMaxCoordsInWorld = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        gameObject.tag = "player";
        engineParticles = body.GetComponent<ParticleSystem>();
        StopEngine();
    }
    
    void Update()
    {
        #region Inputs
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float shootInput = shootAction.ReadValue<float>();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UiManager.instance.SwitchPause();
            return;
        }
        
        #endregion
        
        if (UiManager.instance.isGamePaused())
        {
            return;
        }
        #region Death
        
        if (_isDead)
        {
            _deathTime += Time.deltaTime;
            if (_deathTime >= deathDuration && lives > 0)
            {
                _isDead = false;
                _velocity = Vector2.zero;
                _deathTime = 0;
                transform.position = Vector3.zero;
                _isInvincible = true;
            }

            return;
        }
        #endregion
        
        #region Invincibility
        
        if (_isInvincible)
        {
            _tilInvincibilityEnd += Time.deltaTime;
            _tilNextFlicker += Time.deltaTime;
            if (_tilNextFlicker >= flickeringInterval)
            {
                Flicker();
                _tilNextFlicker = 0;
            }
            if (_tilInvincibilityEnd >= invincibilityDuration)
            {
                _tilInvincibilityEnd = 0;
                _isInvincible = false;
                body.SetActive(true);
            }
        }
        #endregion
        
        #region Movement

        if (moveInput.y != 0)
        {
            StartEngine();
            double rotationY = (transform.rotation.eulerAngles.y * Math.PI) / 180;

            Vector2 rotationDir = new Vector2(
                (float)Math.Cos(rotationY),
                -(float)Math.Sin(rotationY));

            _lastMovingDirection = (_lastMovingDirection + rotationDir).normalized;

            Vector2 velocityChange = Time.deltaTime * accelerationIncreaseRate * _lastMovingDirection;
            if ((_velocity + velocityChange).magnitude < maxSpeed)
            {
                _velocity += velocityChange;
            }
        }
        else
        {
            StopEngine();
            Vector2 velocityBeforeStopping = _velocity;
            _velocity += Time.deltaTime * oppositePower * -_velocity;
            
            if ((int)(Math.Abs(velocityBeforeStopping.x) / velocityBeforeStopping.x) != (int)(Math.Abs(_velocity.x) / _velocity.x) ||
                velocityBeforeStopping.x == 0)
            {
                _velocity.x = 0;
            }
            if ((int)(Math.Abs(velocityBeforeStopping.y) / velocityBeforeStopping.y) != (int)(Math.Abs(_velocity.y) / _velocity.y) ||
                velocityBeforeStopping.y == 0)
            {
                _velocity.y = 0;
            }
            if (_velocity == Vector2.zero)
            {
                _lastMovingDirection = Vector2.zero;
            }
        }
        _rotationY = moveInput.x;
        #endregion
        
        #region Cooldown

        if (_tilNextFire < fireCooldown)
        {
            _tilNextFire += Time.deltaTime;
        }

        if (shootInput == 1.0f && _tilNextFire >= fireCooldown)
        {
            Fire();
            _tilNextFire = 0;
        }
        #endregion
    }

    private void Flicker()
    {
        body.SetActive(!body.activeSelf);
    }
    
    private void Fire()
    {
        BulletObstacle newBullet = Instantiate(bulletPrefab, transform.parent);
        newBullet.transform.position = bulletSpawnPoint.transform.position;
        
        double rotationY = (transform.rotation.eulerAngles.y * Math.PI) / 180;
        Vector2 rotationDir = new Vector2(
            (float)Math.Cos(rotationY),
            -(float)Math.Sin(rotationY));
        Vector2 bulletVelocity = rotationDir * projectileSpeed;
        
        newBullet.Init(bulletVelocity, SizeType.Small);
        newBullet.isPrefab = false;
        newBullet.name += " player ";
        newBullet.transform.eulerAngles = transform.eulerAngles;
        _audioSources[1].Play();
    }
    
    void FixedUpdate()
    {
        if (_isDead || UiManager.instance.isGamePaused())
        {
            return;
        }
        Vector3 rotationVector = new Vector3(0, _rotationY, 0);

        transform.position += new Vector3(_velocity.x, 0, _velocity.y);
        transform.Rotate( Time.deltaTime * rotationSpeed * rotationVector);

        TeleportToScreenBorder();
    }

    void TeleportToScreenBorder()
    {
        var isVisible = IsAnyRendererVisible();
        
        if(isVisible)
        {
            isWrappingX = false;
            isWrappingZ = false;
            return;
        }
        if(isWrappingX && isWrappingZ) {
            return;
        }
        var newPosition = transform.position;
        var viewportPosition = cam.WorldToViewportPoint(newPosition);

        if (!isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0))
        {
            newPosition.x = -newPosition.x;
            isWrappingX = true;
        }
        if (!isWrappingZ && (viewportPosition.y > 1 || viewportPosition.y < 0))
        {
            newPosition.z = -newPosition.z;
            isWrappingZ = true;
        }
        transform.position = newPosition;
    }
    
    protected virtual bool IsAnyRendererVisible()
    {
        foreach(var rend in renderers)
        {
            if(rend.isVisible)
            {
                return true;
            }
        }
        return false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.StartsWith("player") && !_isDead && 
            !other.gameObject.name.Contains("player") && !_isInvincible)
        {
            UiManager.instance.TakeLife();
        }
        
    }

    public void Die()
    {
        _audioSources[0].clip = destructionSound;
        _audioSources[0].loop = false;
        _audioSources[0].Play();
        if (!_isDead)
        {
            body.SetActive(false);
            _isDead = true;
            lives -= 1;
        }
    }
    
    public bool IsDead()
    {
        return _isDead;
    }
    
    public bool IsInvincible()
    {
        return _isInvincible;
    }
    
    public void Reset()
    {
        lives = 3;
        _velocity = Vector2.zero;
        _rotationY = 0;
        _deathTime = 0;
        _tilNextFire = 0;
        _isDead = false;
        _isInvincible = false;
        _tilInvincibilityEnd = 0;
        body.SetActive(true);
        transform.Rotate(Vector3.zero);

        foreach (Transform child in transform.parent.GetComponentsInChildren<Transform>()) {
            if (child.gameObject.CompareTag("bullet"))
            {
                Destroy(child.gameObject);
            }
        }
        
        transform.position = Vector3.zero;
    }

    private void StartEngine()
    {
        if (!engineParticles.isPlaying)
        {
            engineParticles.Play();   
        }

        if (!_audioSources[0].isPlaying)
        {
            _audioSources[0].clip = loopSound;
            _audioSources[0].loop = true;
            _audioSources[0].Play();
        }
    }
    
    private void StopEngine()
    {
        if (!engineParticles.isStopped)
        {
            engineParticles.Stop();   
        }

        if (!_audioSources[0].isPlaying || _audioSources[0].clip == loopSound)
        {
            _audioSources[0].Stop();
            _audioSources[0].loop = false;
        }
    }
}
