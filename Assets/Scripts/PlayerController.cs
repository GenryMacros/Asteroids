using System;
using UnityEngine.InputSystem;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float rotationSpeed = 100f;
    public float accelerationIncreaseRate = 2f;
    public float oppositePower = 0.01f;
    public InputAction playerControls;
    public Camera cam;
    
    private Vector2 _velocity = Vector2.zero;
    private Vector2 _lastMovingDirection = Vector2.zero;
    private float _rotationY; 
    private Vector2 _screenZeroCoordsInWorld;
    private Vector2 _screenMaxCoordsInWorld;
    
    SphereCollider _collider;

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _screenZeroCoordsInWorld = cam.ScreenToWorldPoint(Vector2.zero);
        _screenZeroCoordsInWorld.y *= -1;
        _screenMaxCoordsInWorld = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }
    
    void Update()
    {
        #region Movement
        Vector2 input = playerControls.ReadValue<Vector2>();

        if (input.y != 0)
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
        _rotationY = input.x;
        #endregion

    }
    
    void FixedUpdate()
    {
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
    
    private void OnCollisionEnter(Collision other)
    {
        
    }
}
