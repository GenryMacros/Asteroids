using UnityEditor;
using UnityEngine;


public enum SizeType
{
    Big = 2,
    Medium = 1,
    Small = 0
}


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Obstacle : MonoBehaviour
{
    public GameObject visuals = null;
    public Camera cam;
    
    protected SizeType _sizeType = SizeType.Medium;
    protected Vector2 _velocity;
    protected SphereCollider _collider;
    protected Rigidbody _rigidbody;
    protected Vector2 _screenZeroCoordsInWorld;
    protected Vector2 _screenMaxCoordsInWorld;

    public void Init(Vector2 initialVelocity, SizeType sizeType)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _velocity = initialVelocity;
        _sizeType = sizeType;
        _rigidbody.useGravity = false;
    }

    void Start()
    {
        _screenZeroCoordsInWorld = cam.ScreenToWorldPoint(Vector2.zero);
        _screenZeroCoordsInWorld.y *= -1;
        _screenMaxCoordsInWorld = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }
    
    void Update()
    {
        
    }
}
