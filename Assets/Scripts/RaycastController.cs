using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]

/*
 This script is used to detect the collision in our game by using raycasting
	 */
public class RaycastController : MonoBehaviour
{

	public LayerMask collisionMask;

	const float skinWidth = .015f;      //Rays are fiered from a small width inside of the player
	public int horizontalRayCount = 4;  //Number of horizontal rays
	public int verticalRayCount = 4;    //Number of vertical rays

	[HideInInspector]
	public float horizontalRaySpacing;     //The horizontal space between the rays
	[HideInInspector]
	public float verticalRaySpacing;       //The vertical space between the rays

	[HideInInspector]
	public BoxCollider2D collider;         //Collider component of our player object
	
	RaycastOrigins raycastOrigins;		   //All the infromation about raycasts stored into a data type.


	public CollisionInfo collisions;
	public virtual void Awake()
	{
		collider = GetComponent<BoxCollider2D>();
	}
	public virtual void Start()
	{
		
		CalculateRaySpacing();
		collisions.faceDir = 1;
	}

	/*
	 * Move 
	*/
	public void Move(Vector3 velocity)
	{
		UpdateRaycastOrigins();
		collisions.Reset();

		if (velocity.x != 0)
		{
			collisions.faceDir = (int)Mathf.Sign(velocity.x);
		}
		HorizontalCollisions(ref velocity);
	
		if (velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}

		transform.Translate(velocity);
	}


	/*
	 * This function check horizontal collisions and stores the info.
	 * It also changes the horizontal velocity to a constant value while the ray is hitting the object
	*/
	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		if(Mathf.Abs(velocity.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);			
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit)
			{
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}

		}
	}

	/*
	 * This function check vertical collisions and stores the info.
	 * It also changes the vertical velocity to a constant value while the ray is hitting the ground/object
	*/
	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;		 //Sets raycast origin based on the direction of our velociy vector.
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);  //Draw the raycast and check for collisions.

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit)
			{
				velocity.y = (hit.distance - skinWidth) * directionY;		//Set velocity to a constant value
				rayLength = hit.distance;									//shrink the ray length

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

	/*
	 * Updates the raycast information with the necessary.  
	*/
	void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		//Divide equally the space between rays  
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);		
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;
		public int faceDir;

		public void Reset()
		{
			above = below = false;
			left = right = false;
		}

	}

}