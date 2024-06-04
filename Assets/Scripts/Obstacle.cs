using System.Collections;
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
[RequireComponent(typeof(AudioSource))]
public class Obstacle : MonoBehaviour
{
    public Camera cam;
    public bool isPrefab = false;
    public float speed = 0;
    
    protected SizeType _sizeType = SizeType.Medium;
    protected Vector2 _velocity;
    protected SphereCollider _collider;
    protected Rigidbody _rigidbody;
    protected MeshRenderer renderer;
    protected AudioSource audioSource;
    
    private Renderer[] renderers;
    private bool isWrappingX = false;
    private bool isWrappingZ = false;

    public void Init(Vector2 initialVelocity, SizeType sizeType)
    {
        audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _collider.isTrigger = true;
        _velocity = initialVelocity;
        _sizeType = sizeType;
        _rigidbody.useGravity = false;
        ConfigureScale();
    }

    protected virtual void ConfigureScale()
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
        renderers = GetComponentsInChildren<Renderer>();
        renderer = GetComponent<MeshRenderer>();
    }

    protected virtual bool IsAnyRendererVisible()
    {
        foreach (var rend in renderers)
        {
            if (rend.isVisible)
            {
                return true;
            }
        }

        return false;
    }

    protected void TeleportToScreenBorder()
    {
        Vector2 positionOnScreen = cam.WorldToScreenPoint(transform.position);
        
        if (positionOnScreen.x > Screen.width || positionOnScreen.y > Screen.height ||
            positionOnScreen.x < -Screen.width * 0.3 || positionOnScreen.y < -Screen.height * 0.3 )
        {
            Disappear();
        }
        
        var isVisible = IsAnyRendererVisible();

        if (isVisible)
        {
            isWrappingX = false;
            isWrappingZ = false;
            return;
        }

        if (isWrappingX && isWrappingZ)
        {
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

    protected virtual void Disappear()
    {
    }
    
    protected IEnumerator PlayAudioAndDestroy()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>()) {
            if (child.CompareTag("model"))
            {
                child.gameObject.SetActive(false);
            }
        }
        audioSource.Play();
        yield return new WaitUntil(() => audioSource.time >= audioSource.clip.length);
        Destroy(gameObject);
    }

}
