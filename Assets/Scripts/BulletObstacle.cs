using Siccity.GLTFUtility;
using UnityEngine;

public class BulletObstacle : Obstacle
{

    public float existTime = 1.0f;
   
    private float _timeToDeath = 0.0f;
    
    void Start()
    {
        base.Start();
        if (!visuals)
        {
            visuals = Importer.LoadFromFile(Application.dataPath + "/Visuals/bullet_placeholder.glb");
            visuals.transform.position = transform.position;
            visuals.transform.parent = transform;
        }
        if (isPrefab)
        {
            gameObject.tag = "prefab";
        }
        else
        {
            gameObject.tag = "bullet";
        }
    }
    
    protected override bool IsAnyRendererVisible() 
    {
        return renderer.isVisible;
    }
    
    void FixedUpdate()
    {
        if (!isPrefab)
        {
            transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
            TeleportToScreenBorder();   
            
            _timeToDeath += Time.deltaTime;
            if (_timeToDeath >= existTime)
            {
                Destroy(gameObject);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet") || 
            other.gameObject.CompareTag("alien") || 
            other.gameObject.CompareTag("rock"))
        {
            Destroy(gameObject);
        } else if (other.gameObject.CompareTag("player") && !gameObject.GetComponent<PlayerController>().IsDead())
        {
            other.gameObject.GetComponent<PlayerController>().Die();
            Destroy(gameObject);
        }
    }
}
