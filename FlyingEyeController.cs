using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;
    EnemyController enemyController;

    bool isFacingRight;
    
    //Equation Variables
    bool isFollowingPath;
    Vector3 pathStartPoint;
    Vector3 pathEndPoint;
    Vector3 pathMidPoint;
    float pathTimeStart;

    public float bezierTime = 1f;
    public float bezierDistance = 1f;
    public Vector3 bezierHeight = new Vector3(0, 0.8f, 0); 

    public enum MoveDirections {Left, Right };
    [SerializeField] MoveDirections moveDirection = MoveDirections.Left;

    void Awake()
    {
        //initialization code 
        enemyController = GetComponent<EnemyController>();
        animator = enemyController.GetComponent<Animator>();
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
        

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.freezeEnemy)
        {
            pathTimeStart += Time.deltaTime; 
            return; 
        }

        animator.Play("Flying_eye"); 

        if(!isFollowingPath)
        {
            //sets it on the path to follow 
            float distance = (isFacingRight) ? bezierDistance : -bezierDistance;
            pathStartPoint = rb2d.transform.position;
            pathEndPoint = new Vector3(pathStartPoint.x + distance, pathStartPoint.y, pathStartPoint.z);
            pathMidPoint = pathStartPoint + (((pathEndPoint - pathStartPoint)/2) + bezierHeight);
            pathTimeStart = Time.time;
            isFollowingPath = true; 
        }
        else
        {
            //Creates that wave movement if already on a path 
            float percentage = (Time.time - pathTimeStart) / bezierTime;
            rb2d.transform.position = UtilityFunctions.CalculateQuadraticBezierPoint(pathStartPoint, pathMidPoint, pathEndPoint, percentage); 
            if(percentage >= 1f)
            {
                // Causes the max hieght to flip creating that wave effect 
                bezierHeight *= -1;
                isFollowingPath = false; 
            }
        }
    }

    public void SetMoveDirection(MoveDirections direction)
    {
        //allows dynamic change to movement during game.Just and incase. 
        moveDirection = direction;
        if (moveDirection == MoveDirections.Left)
        {
            if (isFacingRight)
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

    public void ResetFollowingPath()
    {
        //Used to reset the path once the character is outside the camera 
        isFollowingPath = false; 
    }
}
