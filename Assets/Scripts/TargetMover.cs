using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMover : MonoBehaviour
{
	public float speed;

	GameObject target;

	public void SetTarget(GameObject obj)
	{
		target = obj;
	}

	private void Update()
	{
		if (target)
		{
			transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
		}
	}
}
