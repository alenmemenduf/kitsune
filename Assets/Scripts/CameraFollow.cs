using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

	public RaycastController target;
	public Vector2 focusAreaSize;

	public float verticalOffset;

	FocusAreas focusArea;


	void Start()
	{
		focusArea = new FocusAreas(target.collider.bounds, focusAreaSize);
	}

	void LateUpdate()
	{
		focusArea.Update(target.collider.bounds);

		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, .5f);
		Gizmos.DrawCube(focusArea.centre, focusAreaSize);
	}

	struct FocusAreas
	{
		public Vector2 centre;
		public Vector2 velocity;
		float left, right;
		float top, bottom;


		public FocusAreas(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left + right) / 2, (top + bottom) / 2);
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left + right) / 2, (top + bottom) / 2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}

}