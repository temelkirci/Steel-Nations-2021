using UnityEngine;
using System.Collections;

/// <summary>
/// Spin the object at a specified speed
/// </summary>
public class SpinFree : MonoBehaviour {
	[Tooltip("Spin: Yes or No")]
	public bool spin;
	[Tooltip("Spin the parent object instead of the object this script is attached to")]
	public bool spinParent;
	public float speed = 10f;

	public bool clockwise = true;
	public float direction = 1f;
	public float directionChangeSpeed = 2f;

	public bool left = false;
	public bool up = false;
	public bool down = true;

	// Update is called once per frame
	void Update() 
	{
		if (direction < 1f) 
		{
			direction += Time.deltaTime / (directionChangeSpeed / 2);
		}

		if (spin) 
		{
			if (clockwise) 
			{
				if (spinParent)
					transform.parent.transform.Rotate(GetDirection(), (speed * direction) * Time.deltaTime);
				else
					transform.Rotate(GetDirection(), (speed * direction) * Time.deltaTime);
			} 
			else 
			{
				if (spinParent)
					transform.parent.transform.Rotate(-GetDirection(), (speed * direction) * Time.deltaTime);
				else
					transform.Rotate(-GetDirection(), (speed * direction) * Time.deltaTime);
			}
		}
	}

	public Vector3 GetDirection()
    {
		if (left == true)
			return Vector3.left;
		else if (down == true)
			return Vector3.forward;
		else if (up == true)
			return Vector3.up;

		return Vector3.up;
	}
}