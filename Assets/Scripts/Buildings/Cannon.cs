using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
	public Detector range;
	public GameObject projectilePrefab;

	public int consumption;
	public float reloadSpeed;

	Constructible constructible;
	float delay = 0.0f;

	private void Start()
	{
		constructible = GetComponent<Constructible>();
	}

	private void Update()
	{
		delay -= Time.deltaTime;
		if (constructible.GetIsland() && CanFire())
		{
			var closest = range.GetClosest();
			if (closest)
			{
				Fire(closest);
				delay = reloadSpeed;
			}
		}
	}

	bool CanFire()
	{
		return delay < 0.0f && constructible.GetIsland().CanUseResource(Island.ResourceType.Ammo, consumption);
	}

	void Fire(GameObject target)
	{
		constructible.GetIsland().UseResource(Island.ResourceType.Ammo, consumption);

		var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
		projectile.GetComponent<TargetMover>().SetTarget(target);
	}
}
