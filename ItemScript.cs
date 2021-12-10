using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ItemScript : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite; 

    public enum ItemTypes
    {
        Nothing, 
        Random, 
        LifeEnergyBig,
        LifeEnergySmall
    }

    [SerializeField] ItemTypes itemType;

    [SerializeField] bool animate;
    [SerializeField] float destroyDelay;
    [SerializeField] int lifeEnergy;


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
        Animate(animate);
        
        if(destroyDelay > 0)
        {
            SetDestroyDelay(destroyDelay); 
        }

    }

    public void Animate(bool animate)
    {
        if(animate)
        {
            animator.Play("Default");
            animator.speed = 1; 
        }
        else
        {
            animator.Play("Default", 0, 0);
            animator.speed = 0;
        }
    }

    public void SetDestroyDelay(float delay)
    {
        Destroy(gameObject, delay); 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>(); 

            if(lifeEnergy > 0)
            {
                player.ApplyLifeEnergy(lifeEnergy); 
            }
        }
    }
  
    // Update is called once per frame
    void Update()
    {
        
    }
}
