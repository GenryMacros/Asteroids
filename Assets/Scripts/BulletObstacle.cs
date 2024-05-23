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
    
    void Update()
    {
        
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
            other.gameObject.CompareTag("player") || 
            other.gameObject.CompareTag("rock"))
        {
            Destroy(gameObject);
        }
    }
}
