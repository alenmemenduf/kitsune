using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (RaycastController))]
public class Player : MonoBehaviour
{
    //Movement constants
    public float jumpHeight = 4;            //Max height the player can jump
    public float timeToJumpApex = .4f;      //How much time the jump action takes to reach apex
    public float moveSpeed = 6f;             //Player movement speed
    public float dashSpeed = 30f;            //Player dash speed
    public float dashTime = 10f;
    float accelerationTimeAirborne = .25f;   //In air acceleration
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
    public float wallStickTime = 0.25f;      //Max time before player gets unstick from the wall (makes wall leaping easier to perform)
    float timeToWallUnstick;

    //Wall falling when sticky constants/variables
    public float fallTime = 0.5f;
    float timeUntilFall; // timeUntilFall before I unstick and player fall

    //Kinematic operation variables/constants

    [HideInInspector]
    public Vector3 velocity;
    float gravity;
    float jumpVelocity = 8;
    float velocityXSmoothing;

    [HideInInspector]
    public float faceDirection;
    RaycastController controller;

    [HideInInspector]
    public bool isDashing = false;

    float timeLeft;

    ///
    /// debug
    LineRenderer line;
    /// 
    void Dash()
    {
        isDashing = true;
        velocity.x = faceDirection * dashSpeed / dashTime;
   
    }

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        if (!line)
            line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        controller = GetComponent<RaycastController>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);     //Kinematic operatin defining deltaMovement (gravity is treated as acceleartion here)
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;             //kinematic operation of jump velocity

        StartCoroutine("FindTargetsWithDelay", .2f);
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
    // Update is called once per frame
    void Update()
    {
       
        //Movement Input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));  //Get Input from user
        if (input.x != 0)
            faceDirection = input.x; // stores last direction the player faced.
        int wallDirX = (controller.collisions.left) ? -1 : 1;                                       //Direction of the wall we colided with.

        //Horizontal movement smoothing
        float targetVelocityX = input.x * moveSpeed;    //Desired velocity we want to achieve when moving
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);    //Smooth the movement between initial velocity and desired velosity (acceleration is taken into account)

        //Wall sliding
        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && // If player touching either wall &&
            !controller.collisions.below && velocity.y < 0)  //If player is touching the wall while mid air
        {

            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax && timeUntilFall > 0)
            {
                velocity.y = -wallSlideSpeedMax;    // if object falls faster than max wall slide speed while 
                timeUntilFall -= Time.deltaTime;
            }

            if (timeToWallUnstick > 0)  //Time how much time before player can unstick from the wall (0.25 secs)         
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
        //If player is on ground just set its velocity to 0
        if (controller.collisions.above || controller.collisions.below)
        {
            timeUntilFall = fallTime; // if on floor or hit the hed you can slide again
            velocity.y = 0;
        }


        //Jump Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallSliding)
            {
                if (input.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x; //If we are not moving and jump while wallsliding then jump off the wall 
                    velocity.y = wallJumpOff.y;
                } else if (-wallDirX == input.x)
                {
                    timeUntilFall = fallTime; // if jumping and sliding on wall and changing direction => you are able to slide again
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if (controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }

        }

        if (Input.GetMouseButtonDown(0))
        {
            if(visibleTargets.Count != 0)
            {
                float distance = (transform.position - visibleTargets[0].position).magnitude;
                isHooked = true;
                             
                //velocity -= transform.position - visibleTargets[0].position;

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHooked = false;
        }
        if (visibleTargets.Count == 0)
        {
            isHooked = false;
        }



        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
            timeLeft = dashTime;
        }
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }

        velocity.y += gravity * Time.deltaTime;
        if (isHooked)
        {
            /*
            float distance = (transform.position - visibleTargets[0].position).magnitude;
            float maxDistance = 7;
            ;
            if (distance > maxDistance)
            {
                Vector3 test = visibleTargets[0].position + (transform.position - visibleTargets[0].position).normalized * maxDistance;

                float dx = (test.x - transform.position.x) / Time.deltaTime;
                float dy = (test.y - transform.position.y) / Time.deltaTime;

               
                
               
                velocity.x = (-faceDirection * 15 * Vector2.Perpendicular((Vector2)visibleTargets[0].position - (Vector2)transform.position).normalized).x;
                
               
                velocity.y = dy;

            }
            */
            /*
            float distance = (transform.position + velocity - visibleTargets[0].position).magnitude;
            float maxDistance = 5;
            if (distance > maxDistance)
            {
                Vector3 test = visibleTargets[0].position + (transform.position + velocity - visibleTargets[0].position).normalized * maxDistance;
                velocity += (test - (visibleTargets[0].position + (transform.position + velocity - visibleTargets[0].position)));
            }*/
            float dx = (visibleTargets[0].position.x - transform.position.x);
            float dy = (visibleTargets[0].position.y - transform.position.y);

            float distance = (transform.position + velocity - visibleTargets[0].position).magnitude;
            float maxDistance = 4;
            Vector3 v = new Vector3(0, 0, 0);
            if (distance > maxDistance)
            {
                float dx1 = dx / distance * maxDistance;
                float dy1 = dy / distance * maxDistance;
                v.x -= (dx1 - dx) * Time.deltaTime;
                v.y -= (dy1 - dy) * Time.deltaTime;
            }

            velocity.x += v.x / (Time.deltaTime * 5);
            velocity.y += v.y / (Time.deltaTime * 5);

            line.enabled = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, visibleTargets[0].position);
        }
        else
        {
            line.enabled = false;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleHinges();
        }
    }
    private void FindVisibleHinges()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, hingeViewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            visibleTargets.Add(target);
        }
    }
}
