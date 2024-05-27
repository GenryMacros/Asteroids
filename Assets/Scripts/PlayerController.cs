using System;
using UnityEngine.InputSystem;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
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
    public Camera cam;
    public BulletObstacle bulletPrefab;
    public GameObject bulletSpawnPoint;
    public MeshRenderer body;
    public int lives = 3;
    
    public InputAction moveAction;
    public InputAction shootAction;
    
    private Vector2 _velocity = Vector2.zero;
    private Vector2 _lastMovingDirection = Vector2.zero;
    private float _rotationY; 
    private Vector2 _screenZeroCoordsInWorld;
    private Vector2 _screenMaxCoordsInWorld;
    private float _tilNextFire = 0.0f;
    private float _deathTime = 0.0f;
    private bool _isDead = false;
    
    private SphereCollider _collider;
    private Rigidbody _rigidbody;
    
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
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _screenZeroCoordsInWorld = cam.ScreenToWorldPoint(Vector2.zero);
        _screenZeroCoordsInWorld.y *= -1;
        _screenMaxCoordsInWorld = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        gameObject.tag = "player";
    }
    
    void Update()
    {
        #region Death
        
        if (_isDead)
        {
            _deathTime += Time.deltaTime;
            if (_deathTime >= deathDuration && lives > 0)
            {
                _isDead = false;
                body.enabled = true;
                _deathTime = 0;
                transform.position = Vector3.zero;
            }

            return;
        }
        #endregion
        
        #region Inputs
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float shootInput = shootAction.ReadValue<float>();
        #endregion
        
        #region Movement

        if (moveInput.y != 0)
        {
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
    }
    
    void FixedUpdate()
    {
        if (_isDead)
        {
            return;
        }
        Vector3 rotationVector = new Vector3(0, _rotationY, 0);

        transform.position += new Vector3(_velocity.x, 0, _velocity.y);
        transform.Rotate( Time.deltaTime * rotationSpeed * rotationVector);

        TeleportToScreenBorder();
    }

    private void TeleportToScreenBorder()
    {
        Vector3 currentPosition = transform.position;
        Vector2 screenCoordinates = cam.WorldToScreenPoint(currentPosition);

        if (screenCoordinates.x > Screen.width)
        {
            transform.position = new Vector3(_screenZeroCoordsInWorld.x, currentPosition.y, currentPosition.z);
        } else if (screenCoordinates.x < 0)
        {
            transform.position = new Vector3(_screenMaxCoordsInWorld.x, currentPosition.y, currentPosition.z);
        }

        if (screenCoordinates.y > Screen.height)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, _screenZeroCoordsInWorld.y);
        } else if (screenCoordinates.y < 0)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, _screenMaxCoordsInWorld.y);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.StartsWith("player") && !_isDead)
        {
            UiManager.instance.TakeLife();
        }
        
    }

    public void Die()
    {
        if (!_isDead)
        {
            body.enabled = false;
            _isDead = true;
            lives -= 1;
        }
    }
    
    public bool IsDead()
    {
        return _isDead;
    }
    
    public void Reset()
    {
        lives = 3;
        _velocity = Vector2.zero;
        _rotationY = 0;
        _deathTime = 0;
        _tilNextFire = 0;
        _isDead = false;
        body.enabled = true;
        transform.Rotate(Vector3.zero);

        foreach (Transform child in transform.parent.GetComponentsInChildren<Transform>()) {
            if (child.gameObject.CompareTag("bullet"))
            {
                Destroy(child.gameObject);
            }
        }
        
        transform.position = Vector3.zero;
    }
}
