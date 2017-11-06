using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementOrientation : MonoBehaviour
{
	private Rigidbody2D body;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (body.velocity.magnitude > 0.1f)
			transform.rotation = Quaternion.FromToRotation(Vector2.right, body.velocity);
		body.angularVelocity = 0.0f;
	}
}
