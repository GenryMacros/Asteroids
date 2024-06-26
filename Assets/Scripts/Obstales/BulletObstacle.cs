using UnityEngine;


public class BulletObstacle : Obstacle
{

    public float existTime = 1.0f;
    
    void Start()
    {
        base.Start();
        gameObject.tag = "bullet";
        Invoke(nameof(Die), existTime);
    }
    
    protected override void ConfigureScale()
    {
        switch (_sizeType)
        {
            case SizeType.Big:
                gameObject.transform.localScale = new Vector3(3, 3, 3);
                break;
            case SizeType.Medium:
                gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
            case SizeType.Small:
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
        }
    }
    
    protected override bool IsAnyRendererVisible() 
    {
        return renderer.isVisible;
    }
    
    void FixedUpdate()
    {
        if (UiManager.instance.isGamePaused())
        {
            return;
        }
        
        transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.deltaTime;
        TeleportToScreenBorder();   
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet") || 
            other.gameObject.CompareTag("alien") || 
            other.gameObject.CompareTag("rock"))
        {
            Destroy(gameObject);
        } else if (other.gameObject.CompareTag("player") && 
                   !other.gameObject.GetComponent<PlayerController>().IsDead() &&
                   !other.gameObject.GetComponent<PlayerController>().IsInvincible())
        {
            if (gameObject.name.Contains("player"))
            {
                return;
            }
            other.gameObject.GetComponent<PlayerController>().Die();
            Destroy(gameObject);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
