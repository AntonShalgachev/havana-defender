using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
	static float Epsilon = 0.1f;

	public float speed;
	public float damage;

	GameObject target;

	public void SetTarget(GameObject obj)
	{
		target = obj;
	}

	private void Update()
	{
		if (target)
		{
			var targetPos = target.transform.position;
			var pos = transform.position;

			if ((targetPos - pos).magnitude < Epsilon)
			{
				OnArrived();
			}
			else
			{
				transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
			}
		}
	}

	void OnArrived()
	{
		target.GetComponent<Health>().TakeDamage(damage);
		Debug.Log("Dealing damage");
		Destroy(gameObject);
	}
}
