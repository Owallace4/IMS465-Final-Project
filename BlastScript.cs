using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastScript : MonoBehaviour
{
    Animator animator; 
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    float destroyTime;

    bool freezeBlast;

    RigidbodyConstraints2D rb2dConstraints; 

    public int damage = 1;

    [SerializeField] float blastSpeed;
    [SerializeField] Vector2 blastDirection;
    [SerializeField] float destroyDelay; 

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeBlast) return; 

        //remove the blast once its time's up 
        destroyTime -= Time.deltaTime; 
        if(destroyTime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetBlastSpeed(float speed)
    {
        this.blastSpeed = speed; 
    }

    public void SetBlastDirection(Vector2 direction)
    {
        this.blastDirection = direction; 
    }

    public void SetDamageValue(int damage)
    {
        this.damage = damage; 
    }

    public void SetDestroyDelay(float delay)
    {
        this.destroyDelay = delay; 
    }

    public void Shoot()
    {
        sprite.flipX = (blastDirection.x < 0);
        rb2d.velocity = blastDirection * blastSpeed;
        destroyTime = destroyDelay;
        animator.Play("Blast");
    }

    public void FreezeBlast(bool freeze)
    {
        if (freeze)
        {
            freezeBlast = true;
            rb2dConstraints = rb2d.constraints;
            animator.speed = 0;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            rb2d.velocity = Vector2.zero; 
        }
        else
        {
            freezeBlast = false;
            animator.speed = 1;
            rb2d.constraints = rb2dConstraints;
            rb2d.velocity = blastDirection * blastSpeed; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.TakeDamage(this.damage); 
            }
            Destroy(gameObject, 0.01f);
        }
    }

}
