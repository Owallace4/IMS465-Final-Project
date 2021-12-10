using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class EnemyController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite; 

    bool isInvincible;

    GameObject explodeEffect;

    RigidbodyConstraints2D rb2dConstraints;

    public bool freezeEnemy;

    public int scorePoints = 500; 
    public int currentHealth;
    public int maxHealth = 1;
    public int contactDamage = 1;
    public int explosionDamage = 0;

    [Header("Bonus Item Settings")]
    public ItemScript.ItemTypes bonusItemTypes;
    public float bonusDestroyDelay = 5f;
    public Vector2 bonusVelocity = new Vector2(0, 3f); 

    [SerializeField] GameObject explodeEffectPrefab;

    void Awake()
    {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();


    }

    // Start is called before the first frame update
    void Start()
    {
        
        currentHealth = maxHealth; 
    }

    public void Flip()
    {
        transform.Rotate(0, 180f, 0); 
    }

    public void Invincible(bool invinciblility)
    {
        isInvincible = invinciblility; 
    }

    public void TakeDamage(int damage)
    {
        if(!isInvincible)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); 
            if(currentHealth <= 0)
            {
                Defeat(); 
            }
        }
    }

    void StartDefeatAnimation()
    {
        explodeEffect = Instantiate(explodeEffectPrefab);
        explodeEffect.name = explodeEffectPrefab.name;
        explodeEffect.transform.position = sprite.bounds.center;
        explodeEffect.GetComponent<ExplosionScript>().SetDamageValue(this.explosionDamage);
        Destroy(explodeEffect, 2f);
        /*
        GameObject bonusItemPrefab = GameManager.Instance.GetBonusItem(bonusItemTypes); 
        if(bonusItemPrefab != null)
        {
            GameObject bonusItem = Instantiate(bonusItemPrefab);
            bonusItem.name = bonusItemPrefab.name;
            bonusItem.transform.position = explodeEffect.transform.position;
            bonusItem.GetComponent<ItemScript>().Animate(true);
            bonusItem.GetComponent<ItemScript>().SetDestroyDelay(bonusDestroyDelay);
            bonusItem.GetComponent<Rigidbody2D>().velocity = bonusVelocity; 
        } */ 
    }

    void StopDefeatAnimation()
    {
        Destroy(explodeEffect); 
    }

    void Defeat()
    {
        StartDefeatAnimation(); 
        Destroy(gameObject);
        GameManager.Instance.AddScorePoints(this.scorePoints); 
    }

    public void FreezeEnemy(bool freeze)
    {
        if(freeze)
        {
            freezeEnemy = true;
            animator.speed = 0;
            rb2dConstraints = rb2d.constraints;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll; 
        }
        else 
        {
            freezeEnemy = false;
            animator.speed = 1;
            rb2d.constraints = rb2dConstraints; 
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")) 
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.HitSide(transform.position.x > player.transform.position.x);
            player.TakeDamage(this.contactDamage); 
        }
    }
}
