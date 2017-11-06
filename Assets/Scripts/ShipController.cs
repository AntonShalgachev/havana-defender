using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	public Detector range;
	public GameObject projectilePrefab;
	public float reloadSpeed;

	Vector2 target;
	Movement movement;
	float delay = 0.0f;
	bool boarded = false;

	private void Start()
	{
		movement = GetComponent<Movement>();
	}

	private void Update()
	{
		target = GameController.Instance.GetIsland().gameObject.transform.position;

		var dir = target - (Vector2)transform.position;
		movement.SetDirection(dir);

		delay -= Time.deltaTime;
		if (delay < 0.0f)
		{
			var closest = range.GetClosest();
			if (closest)
			{
				Fire(closest);
				delay = reloadSpeed;
			}
		}
	}

	void Fire(GameObject target)
	{
		var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
		projectile.GetComponent<Cannonball>().SetTarget(target);
	}

	private void OnDestroy()
	{
		if (GameController.Instance)
			GameController.Instance.OnShipDestroyed(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Island") && !boarded)
		{
			boarded = true;
			GameController.Instance.OnShipBoardedFort(gameObject);

			Destroy(gameObject);
		}
	}
}
