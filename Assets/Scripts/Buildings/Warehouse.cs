using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
	public float rate;

	Constructible constructible;
	float accumulated = 0.0f;

	private void Start()
	{
		constructible = GetComponent<Constructible>();
	}

	private void Update()
	{
		if (constructible.GetIsland())
		{
			accumulated += rate * Time.deltaTime;
			AddPendingResources();
		}
	}

	void AddPendingResources()
	{
		Debug.Assert(constructible.GetIsland());

		var amountToAdd = Mathf.FloorToInt(accumulated);
		accumulated -= amountToAdd;
		constructible.GetIsland().AddResource(Island.ResourceType.Ammo, amountToAdd);
	}
}
