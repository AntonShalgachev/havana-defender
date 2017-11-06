using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	public Island island;

	Vector2 target;
	Movement movement;

	private void Start()
	{
		movement = GetComponent<Movement>();
	}

	private void Update()
	{
		target = island.gameObject.transform.position;

		var dir = target - (Vector2)transform.position;
		movement.SetDirection(dir);
	}
}
