using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
	public float maxSpeed;
	public float maxForce;
	public float forceScale;

	Rigidbody2D rigidBody;
	Vector2 dir;
	float mult;

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		mult = 1.0f;
	}

	private void FixedUpdate()
	{
		var velocity = rigidBody.velocity;
		var newVelocity = dir * maxSpeed * mult;
		var force = (newVelocity - velocity) * forceScale;
		force = Vector2.ClampMagnitude(force, maxForce);

		rigidBody.AddForce(force);
	}

	public void SetDirection(Vector2 dir)
	{
		this.dir = dir.normalized;
	}

	public void SetVelocityMultiplier(float mult)
	{
		this.mult = mult;
	}

	public float GetVelocity()
	{
		return rigidBody.velocity.magnitude;
	}
}
