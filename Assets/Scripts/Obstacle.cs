using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class Obstacle : MonoBehaviour
{
    public Camera cam;
    
    protected Vector2 _velocity;
    protected SphereCollider _collider;
    protected Vector2 _screenZeroCoordsInWorld;
    protected Vector2 _screenMaxCoordsInWorld;
    
    public Obstacle(Vector2 initialVelocity)
    {
        _velocity = initialVelocity;
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
        
    }
}
