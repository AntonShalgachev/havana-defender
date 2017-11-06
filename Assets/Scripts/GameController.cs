using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	public float tileSize;
	public Island island;

	public GameObject cannonPrefab;
	public GameObject warehousePrefab;
	public GameObject barracksPrefab;

	public UIResource uiGoldResource;
	public UIResource uiAmmoResource;
	public UIResource uiSoldiersResource;

	GameObject currentBuilding = null;

	private void Awake()
	{
		Debug.Assert(Instance == null);
		Instance = this;
	}

	private void Start()
	{
		island.InitRandom();
	}

	private void Update()
	{
		uiGoldResource.SetValue(island.GetResourceAmount(Island.ResourceType.Gold));
		uiAmmoResource.SetValue(island.GetResourceAmount(Island.ResourceType.Ammo));
		uiSoldiersResource.SetValue(island.GetResourceAmount(Island.ResourceType.Soldier));

		if (currentBuilding)
		{
			var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			currentBuilding.transform.position = new Vector3(pos.x, pos.y, currentBuilding.transform.position.z);

			island.HighlightCellsForConstructible(currentBuilding);
		}

		if (Input.GetMouseButtonDown(0) && currentBuilding != null)
		{
			if (island.CanBuildConstructible(currentBuilding))
			{
				island.BuildConstructible(currentBuilding);
			}
			else
			{
				DestroyObject(currentBuilding);
			}

			currentBuilding = null;
			island.UnhighlightAll();
		}

		if (Input.GetButtonDown("Cancel") && currentBuilding != null)
		{
			DestroyObject(currentBuilding);

			currentBuilding = null;
			island.UnhighlightAll();
		}
	}

	GameObject GetPrefab(BuildingType type)
	{
		switch (type)
		{
			case BuildingType.Cannon:
				return cannonPrefab;
			case BuildingType.Warehouse:
				return warehousePrefab;
			case BuildingType.Barracks:
				return barracksPrefab;
		}

		Debug.Assert(false);
		return null;
	}

	public void OnBuildButtonClick(BuildingType type)
	{
		if (currentBuilding != null)
			return;

		GameObject prefab = GetPrefab(type);

		currentBuilding = Instantiate(prefab, Vector2.zero, Quaternion.identity);
	}
}
