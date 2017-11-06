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
		if (constructible.GetIsland())
		{
			var reloadDt = Time.deltaTime;

			var cannons = constructible.GetIsland().GetNumberOfCannons();
			Debug.Assert(cannons > 0); // We are cannon at least
			var soldiers = constructible.GetIsland().GetResourceAmount(Island.ResourceType.Soldier);
			var reloadMultiplier = (float)soldiers / cannons;
			reloadMultiplier = Mathf.Clamp(reloadMultiplier, 0.0f, 2.0f);

			delay -= reloadDt * reloadMultiplier;

			if (CanFire())
			{
				var closest = range.GetClosest();
				if (closest)
				{
					Fire(closest);
					delay = reloadSpeed;
				}
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
		projectile.GetComponent<Cannonball>().SetTarget(target);
	}

	private void OnDestroy()
	{
		if (!constructible.GetIsland())
			return;

		// Code duplication
		var cannons = constructible.GetIsland().GetNumberOfCannons();
		Debug.Assert(cannons > 0);
		var soldiers = constructible.GetIsland().GetResourceAmount(Island.ResourceType.Soldier);

		var soldiersPerCannon = soldiers / cannons;
		constructible.GetIsland().UseResource(Island.ResourceType.Soldier, soldiersPerCannon);

		Debug.LogFormat("Cannon destroyed, along with {0} soldiers", soldiersPerCannon);
	}
}
