using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d; 
    Rigidbody2D rb2d; 

    float keyHorizontal;
    bool keyJump;
    bool keyShoot;

    bool isGrounded;
    bool isShooting;
    bool isTakingDamage;
    bool isInvincible; 
    bool isFacingRight;

    bool hitSideRight;

    bool freezeInput;
    bool freezePlayer;
    bool freezeBlast; 

    float shootTime;
    bool keyShootRelease;

    RigidbodyConstraints2D rb2dConstraints; 

    public int currentHealth;
    public int maxHealth = 28; 

    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float jumpSpeed = 6f;

    [SerializeField] int blastDamage = 1;
    [SerializeField] float blastSpeed = 5f;
    [SerializeField] Transform blastShootPos;
    [SerializeField] GameObject blastPrefab; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();

        //Sprite default orientation 
        isFacingRight = true;

        currentHealth = maxHealth; 
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        //ground check 
        Vector3 box_origin = box2d.bounds.center;
        box_origin.y = box2d.bounds.min.y + (box2d.bounds.extents.y /4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        //player box collision detection with ground 
        if (raycastHit.collider != null)
        {
            isGrounded = true; 
           
        }
        //debug ground check 
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_origin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);
    }

    // Update is called once per frame
    void Update()
    {
        if(isTakingDamage)
        {
            animator.Play("Player_Hurt");
            return; 
        }
        PlayerDebugInput(); 
        PlayerDirectionInput();
        PlayerJumpInput(); 
        PlayerShootInput(); 
        PlayerMovement();
       
    }

    void PlayerDebugInput()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            GameObject[] blasts = GameObject.FindGameObjectsWithTag("Blast"); 
            if(blasts.Length > 0)
            {
                freezeBlast = !freezeBlast; 
                foreach (GameObject blast in blasts)
                {
                    blast.GetComponent<BlastScript>().FreezeBlast(freezeBlast); 
                }
            }
            Debug.Log("Freeze Blasts: " + freezeBlast);
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            Invincible(!isInvincible);
            Debug.Log("Invincible: " + isInvincible); 
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            FreezeInput(!freezeInput);
            Debug.Log("Freeze Input: " + freezeInput);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            FreezePlayer(!freezePlayer);
            Debug.Log("Freeze Player: " + freezePlayer);
        }
    }

    void PlayerDirectionInput()
    {
        //Basic player movement input
        if (!freezeInput)
        {
            keyHorizontal = Input.GetAxisRaw("Horizontal");
        }
       

    }

    void PlayerJumpInput()
    {
        //Basic player jumping input
        if(!freezeInput)
        {
            keyJump = Input.GetKeyDown(KeyCode.Space);
        }
        
    }

    void PlayerShootInput()
    {
        //how long your shooting 
        float shootTimeLength = 0;
        //how long the key is pressed 
        float keyShootReleaseTimeLength = 0;

        if (!freezeInput)
        {
            keyShoot = Input.GetKey(KeyCode.J);
        }
        

        if (keyShoot && keyShootRelease)
        {
            isShooting = true;

            keyShootRelease = false;
            shootTime = Time.time;
            //shoot blast 
            Invoke("ShootBlast", 0.1f); 

        }
        if(!keyShoot && !keyShootRelease)
        {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
            
        }
        if(isShooting)
        {
            shootTimeLength = Time.time - shootTime; 
            if(shootTimeLength >= 0.25f || keyShootReleaseTimeLength >= 0.15f)
            {
                isShooting = false;
                
            }
        }
    }

    void PlayerMovement()
    {
        //Basic player movement and jumping with animations woven in
        if (keyHorizontal < 0)
        {
            //flips the sprite
            if (isFacingRight)
            {
                Flip();
            }
            if (isGrounded)
            {
                if (isShooting)
                {
                    animator.Play("Player_Attack");
                }
                else
                {
                    animator.Play("Player_Walk");
                }

            }
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);

        }
        else if (keyHorizontal > 0)
        {
            //flips the sprite
            if (!isFacingRight)
            {
                Flip();
            }
            if (isGrounded)
            {
                if (isShooting)
                {
                    animator.Play("Player_Attack");
                }
                else
                {
                    animator.Play("Player_Walk");
                }
            }
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);

        }
        else
        {
            if (isGrounded)
            {
                if (isShooting)
                {
                    animator.Play("Player_Attack");
                }
                else
                {
                    animator.Play("Player_Idle");
                }
            }
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y);

        }


        if (keyJump && isGrounded)
        {
            if (isShooting)
            {
                animator.Play("Player_Attack");
            }
            else
            {
                animator.Play("Player_Jump");

            }

            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
        }

        if (!isGrounded)
        {
            if (isShooting)
            {
                animator.Play("Player_JumpAttack");
            }
            else
            {
                animator.Play("Player_Jump");

            }

        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f); 
    }

    public void ApplyLifeEnergy(int amount)
    {
        if(currentHealth < maxHealth)
        {
            int healthDiff = maxHealth - currentHealth;
            if (healthDiff > amount) healthDiff = amount;
            StartCoroutine(AddLifeEnergy(healthDiff)); 
        }
    }

    private IEnumerator AddLifeEnergy(int amount)
    {
        for (int i = 0; i < amount; i ++)
        {
            currentHealth++;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UIHealthBar.instance.setValue(currentHealth / (float)maxHealth);
            yield return new WaitForSeconds(0.05f); 
        }
    }

    void ShootBlast()
    {
        // instantiates the blasts at the designated position 
        GameObject blast = Instantiate(blastPrefab, blastShootPos.position, Quaternion.identity);
        // renames the copies to the originals names 
        blast.name = blastPrefab.name;
        blast.GetComponent<BlastScript>().SetDamageValue(blastDamage);
        blast.GetComponent<BlastScript>().SetBlastSpeed(blastSpeed);
        blast.GetComponent<BlastScript>().SetBlastDirection((isFacingRight) ? Vector2.right : Vector2.left);
        blast.GetComponent<BlastScript>().Shoot();
    }

    public void HitSide(bool rightSide)
    {
        hitSideRight = rightSide; 
    }

    //will allow the player to have I frames 
    public void Invincible(bool invinciblility)
    {
        isInvincible = invinciblility;
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UIHealthBar.instance.setValue(currentHealth / (float)maxHealth); 
            if (currentHealth <= 0)
            {
                Defeat();
            }
            else
            {
                StartDamageAnimation(); 
            }
        }
    }

    void StartDamageAnimation()
    {
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            Invincible(true);
            FreezeInput(true); 
            float hitForceX = 0.50f;
            float hitForceY = 1.50f; 
            if(hitSideRight)
            {
                hitForceX = -hitForceX;
                rb2d.velocity = Vector2.zero;
                rb2d.AddForce(new Vector2(hitForceX, hitForceY), ForceMode2D.Impulse); 
            }
        }
    }

    void StopDamageAnimation()
    {
        isTakingDamage = false;
        Invincible(false);
        FreezeInput(false);
        // plays the hurt animation but added a reset so it doesnt get stuck in the animation 
        animator.Play("Player_Hurt", -1, 0f); 
    }



    void Defeat()
    {
        GameManager.Instance.PlayerDefeated(); 
        Destroy(gameObject); 
    }

    public void FreezeInput(bool freeze)
    {
        freezeInput = freeze; 
    }

    public void FreezePlayer(bool freeze)
    {
        if (freeze)
        {
            freezePlayer = true;
            animator.speed = 0;
            rb2dConstraints = rb2d.constraints;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            freezePlayer = false;
            animator.speed = 1;
            rb2dConstraints = rb2d.constraints;
        }
    }

}
