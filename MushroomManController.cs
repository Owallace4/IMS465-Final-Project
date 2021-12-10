using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomManController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    EnemyController enemyController;

    bool isFacingRight;
    bool isGrounded;
    bool isJumping;

    float jumpTimer;
    float jumpDelay = 0.25f;

    int jumpPatternIndex;
    int[] jumpPattern;
    int[][] jumpPatterns = new int[][]
    {
        new int[1] {1 },
        new int[2] {0,1 },
        new int[3] {0,0,1}
    };

    int jumpVelocityIndex;
    Vector2 jumpVelocity;
    Vector2[] jumpVelocities =
    {
        //control the jump distances
        new Vector2(3.0f, 5.0f),// low jump
        new Vector2(2.75f, 7.0f)// high jump 
    };

    [SerializeField] RuntimeAnimatorController racMushroomMan; 

    public enum MoveDirections { Left, Right };
    [SerializeField] MoveDirections moveDirection = MoveDirections.Left;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        animator = enemyController.GetComponent<Animator>();
        box2d = enemyController.GetComponent<BoxCollider2D>();
        rb2d = enemyController.GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        

        isFacingRight = true; 
        if(moveDirection == MoveDirections.Left)
        {
            isFacingRight = false;
            enemyController.Flip(); 
        }

        jumpPattern = null;
        SetAnimatorController(); 
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
        box_origin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        //MushroomMan box collision detection with ground 
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
        if(enemyController.freezeEnemy)
        {
            return; 
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player"); 

        if(isGrounded)
        {
            animator.Play("MushroomMove");
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            jumpTimer -= Time.deltaTime;
            if(jumpTimer < 0)
            {
                if (jumpPattern == null)
                {
                    jumpPatternIndex = 0;
                    jumpPattern = jumpPatterns[Random.Range(0, jumpPatterns.Length)];
                }
                jumpVelocityIndex = jumpPattern[jumpPatternIndex];
                jumpVelocity = jumpVelocities[jumpVelocityIndex];
                if (player.transform.position.x <= transform.position.x)
                {
                    jumpVelocity.x *= -1;
                }
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpVelocity.y);
                jumpTimer = jumpDelay;
                if (++jumpPatternIndex > jumpPattern.Length - 1)
                {
                    jumpPattern = null;
                }
            }
           
        }
        else
        {
            animator.Play("MushroomJump");
            //as opposed to using force by constantly applying x it allows him to clear obstacles 
            rb2d.velocity = new Vector2(jumpVelocity.x, rb2d.velocity.y);
            isJumping = true; 
            if(jumpVelocity.x <= 0)
            {
                if(isFacingRight = !isFacingRight)
                {
                    isFacingRight = !isFacingRight;
                    enemyController.Flip();  
                }
            }
            else
            {
                if (!isFacingRight)
                {
                    isFacingRight = !isFacingRight;
                    enemyController.Flip();
                }
            }
        }
    }

    void SetAnimatorController()
    {
        animator.runtimeAnimatorController = racMushroomMan; 
    }
    
}
