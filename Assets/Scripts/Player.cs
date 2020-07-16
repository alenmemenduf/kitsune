using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (RaycastController))]
public class Player : MonoBehaviour
{

    //Game manager
    [HideInInspector]
    public bool isDead = false;
    public bool isWinner = false;

    //Movement constants
    public float maxJumpHeight = 4;            //Max height the player can jump
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;      //How much time the jump action takes to reach apex
    public float moveSpeed = 6f;             //Player movement speed
    float accelerationTimeAirborne = .2f;   //In air acceleration
    float accelerationTimeGrounded = .1f;   //Grounded acceleration

    //Grappling hook
    public float hingeViewRadius;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();
    public LayerMask targetMask;
    private bool isHooked = false;

    //Wall jump/leap constants
    public Vector2 wallJumpOff;             //Velocity when player jumps off the wall withouth moving
    public Vector2 wallLeap;                //Velocity when player wants to wall leap from curent wall to another in the opposite direction

    //Wall slide constants
    public float wallSlideSpeedMax = 3;     //Max speed that player can have while sliding the wall without moving
    public float wallStickTime = 0.05f;      //Max time before player gets unstick from the wall (makes wall leaping easier to perform)
    float timeToWallUnstick;

    //Wall falling when sticky constants/variables
    public float fallTime = 0.5f;
    float timeUntilFall; // timeUntilFall before I unstick and player fall

    //Animator
    public Animator animator;

    //Kinematic operation variables/constants
    [HideInInspector]
    public Vector3 velocity;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity; 
    float velocityXSmoothing;

    [HideInInspector]
    public float faceDirection;
    RaycastController controller;

    //Dash
    public float dashSpeed = 30f;            //Player dash speed
    public float dashCooldownTime = 2f;
    public bool dashed = false;

    float timeUntilDash;

    private SpriteRenderer renderer;

   


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<RaycastController>();
        renderer = GetComponent<SpriteRenderer>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);          //Kinematic operatin defining deltaMovement (gravity is treated as acceleartion here)
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;                  //kinematic operation of jump velocity
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        timeUntilDash = 0f;
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
    }
    void Update()
    {
        #region MOVEMENT
        //Movement Input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));  // Get Input from user
        if (input.x != 0)
        {
            faceDirection = input.x;    // Stores last direction the player faced.
        }
        animator.SetFloat("Speed", Mathf.Abs(input.x));

        int wallDirX = (controller.collisions.left) ? -1 : 1;    // Direction of the wall we colided with.

        //Horizontal movement smoothing
        float targetVelocityX = input.x * moveSpeed;          // Desired velocity we want to achieve when moving
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);    //Smooth the movement between initial velocity and desired velosity (acceleration is taken into account)
        #endregion

        #region WALL SLIDE
        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && // If player touching either wall &&
            !controller.collisions.below && velocity.y < 0)                // If player is touching the wall while mid air
        {

            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax && timeUntilFall > 0)
            {
                velocity.y = -wallSlideSpeedMax;    // If object falls faster than max wall slide speed while 
                timeUntilFall -= Time.deltaTime;
            }
            
            if (timeToWallUnstick > 0)             //Time how much time before player can unstick from the wall (0.25 secs)         
            {
                velocityXSmoothing = 0;
                velocity.x = 0;
                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        animator.SetBool("isOnWall", wallSliding);
        #endregion

        //If player is on ground just set its velocity to 0
        if (controller.collisions.above || controller.collisions.below)
        {
            timeUntilFall = fallTime; // if on floor or hit the hed you can slide again
            velocity.y = 0;
        }

        #region JUMP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallSliding)
            {
                if (input.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x; // If we are not moving and jump while wallsliding then jump off the wall 
                    velocity.y = wallJumpOff.y;
                } else if (-wallDirX == input.x)
                {
                    timeUntilFall = fallTime;               // If jumping and sliding on wall and changing direction => you are able to slide again
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if (controller.collisions.below)
            {
                velocity.y = maxJumpVelocity;
            }

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        #endregion

        #region DASH


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (timeUntilDash <= 0)
            {
                timeUntilDash = dashCooldownTime;
                velocity.x = faceDirection * dashSpeed;

                animator.SetTrigger("dashed");
              
            }
        }

        if(timeUntilDash > 0)
        {
            timeUntilDash -= Time.deltaTime;

        }

      
        #endregion

        #region FLIP
        if (velocity.x < 0)
        {
            renderer.flipX = true;
        
        }
        else if(velocity.x > 0)
        {
            renderer.flipX = false;
        }
        #endregion

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
