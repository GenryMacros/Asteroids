using UnityEngine;


public enum SizeType
{
    Big = 2,
    Medium = 1,
    Small = 0
}


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Obstacle : MonoBehaviour
{
    public GameObject visuals = null;
    public Camera cam;
    public bool isPrefab = false;
    public float speed = 0;
    public Vector2 direction;
    
    protected SizeType _sizeType = SizeType.Medium;
    protected Vector2 _velocity;
    protected SphereCollider _collider;
    protected Rigidbody _rigidbody;
    protected Vector2 initialDirection;
    
    private MeshRenderer renderer;
    private bool isWrappingX = false;
    private bool isWrappingZ = false;
    
    public void Init(Vector2 initialVelocity, SizeType sizeType)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _velocity = initialVelocity;
        _sizeType = sizeType;
        _rigidbody.useGravity = false;
        ConfigureScale();
    }
    
    protected void ConfigureScale()
    {
        switch (_sizeType)
        {
            case SizeType.Big:
                gameObject.transform.localScale = new Vector3(5, 5, 5);
                break;
            case SizeType.Medium:
                gameObject.transform.localScale = new Vector3(3, 3, 3);
                break;
            case SizeType.Small:
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }

    public void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private bool IsAnyRendererVisible()
    {
        return renderer.isVisible;
    }
    
    void Update()
    {
        
    }
    
    protected void TeleportToScreenBorder()
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
    
}
