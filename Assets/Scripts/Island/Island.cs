using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
	public enum ResourceType
	{
		Gold,
		Ammo,
		Soldier
	}

	struct Point
	{
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public int x;
		public int y;
	}

	public int gridCols;
	public int gridRows;
	
	public int smoothingThreshold;
	public int smoothingIterations;

	public GameObject wallTile;
	public GameObject highlightTile;
	public GameObject tilesObject;
	public GameObject highlightsObject;

	public int initialCurrency;

	private float tileSize;
	private bool[,] currentGrid;
	private GameObject[,] buildings;

	private Dictionary<ResourceType, int> resources;

	public void Start()
	{
		tileSize = GameController.Instance.tileSize;
	}

	public void InitRandom()
	{
		GenerateRandom();
		InitState();
	}

	public void GenerateRandom()
	{
		var gridGenerator = new GridGenerator(gridRows, gridCols);
		gridGenerator.GenerateSmoothRegion(smoothingIterations, smoothingThreshold);

		currentGrid = gridGenerator.grid;
		GenerateTiles();
	}

	void GenerateTiles()
	{
		var width = currentGrid.GetLength(0);
		var height = currentGrid.GetLength(1);

		foreach (Transform child in tilesObject.transform)
			Destroy(child.gameObject);

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (!currentGrid[i, j])
					continue;

				//var pos = GetTileCenter(i, j);
				//var tile = Instantiate(wallTile, pos, Quaternion.identity, tilesObject.transform);

				var tile = SpawnTile(wallTile, i, j, tilesObject.transform);
			}
		}
	}

	GameObject SpawnTile(GameObject prefab, int x, int y, Transform parent, Color color)
	{
		var tile = Instantiate(prefab, GetTileCenter(x, y), Quaternion.identity, parent);

		var renderer = tile.GetComponent<SpriteRenderer>();
		renderer.color = color;
		
		var tileWidth = renderer.bounds.size.x;
		tile.transform.localScale = Vector2.Scale(tile.transform.localScale, Vector2.one * tileSize / tileWidth);

		return tile;
	}

	GameObject SpawnTile(GameObject prefab, int x, int y, Transform parent)
	{
		return SpawnTile(prefab, x, y, parent, Color.white);
	}

	void InitState()
	{
		resources = new Dictionary<ResourceType, int>();
		resources.Add(ResourceType.Gold, initialCurrency);
		resources.Add(ResourceType.Ammo, 0);
		resources.Add(ResourceType.Soldier, 0);

		buildings = new GameObject[gridCols, gridRows];
		for (int i = 0; i < buildings.GetLength(0); i++)
			for (int j = 0; j < buildings.GetLength(1); j++)
				buildings[i, j] = null;
	}

	Vector2 GetTileCenter(int x, int y)
	{
		var width = currentGrid.GetLength(0);
		var height = currentGrid.GetLength(1);
		return new Vector2((x - 0.5f * width) * tileSize, (y - 0.5f * height) * tileSize);
	}

	Vector2 GetSize()
	{
		return new Vector2(gridCols, gridRows) * tileSize;
	}

	public bool IsFree(int x, int y)
	{
		if (x < 0 || x >= gridCols)
			return false;
		if (y < 0 || y >= gridRows)
			return false;

		return currentGrid[x, y] == true && buildings[x, y] == null;
	}

	public void UnhighlightAll()
	{
		foreach (Transform child in highlightsObject.transform)
			Destroy(child.gameObject);
	}

	Point GetTopConstructibleLeftTile(GameObject obj)
	{
		var constructible = obj.GetComponent<Constructible>();
		Debug.Assert(constructible);

		var worldPos = (Vector2)obj.transform.position;

		var size = constructible.GetLogicalSize() * tileSize;
		var topLeft = worldPos - 0.5f * size;

		Vector2 localTopLeft = transform.InverseTransformPoint(topLeft);

		var pos = localTopLeft + GetSize() * 0.5f;

		var tileX = Mathf.CeilToInt(pos.x / tileSize);
		var tileY = Mathf.CeilToInt(pos.y / tileSize);

		return new Point(tileX, tileY);
	}

	public void HighlightCellsForConstructible(GameObject obj)
	{
		var constructible = obj.GetComponent<Constructible>();
		Debug.Assert(constructible);

		UnhighlightAll();
		var topLeft = GetTopConstructibleLeftTile(obj);

		for (int i = 0; i < constructible.width; i++)
		{
			for (int j = 0; j < constructible.height; j++)
			{
				var x = topLeft.x + i;
				var y = topLeft.y + j;

				var color = IsFree(x, y) ? Color.green : Color.red;
				SpawnTile(highlightTile, x, y, highlightsObject.transform, color);
			}
		}
	}

	public bool CanBuildConstructible(GameObject obj)
	{
		var constructible = obj.GetComponent<Constructible>();
		Debug.Assert(constructible);

		var topLeft = GetTopConstructibleLeftTile(obj);
		
		for (int i = 0; i < constructible.width; i++)
		{
			for (int j = 0; j < constructible.height; j++)
			{
				var x = topLeft.x + i;
				var y = topLeft.y + j;

				if (!IsFree(x, y))
					return false;
			}
		}

		return true;
	}

	public void BuildConstructible(GameObject obj)
	{
		if (!CanBuildConstructible(obj))
			return;

		var constructible = obj.GetComponent<Constructible>();
		Debug.Assert(constructible);

		var topLeft = GetTopConstructibleLeftTile(obj);
		for (int i = 0; i < constructible.width; i++)
		{
			for (int j = 0; j < constructible.height; j++)
			{
				var x = topLeft.x + i;
				var y = topLeft.y + j;

				buildings[x, y] = obj;
			}
		}

		constructible.AttachToIsland(this);

		var topLeftCenter = GetTileCenter(topLeft.x, topLeft.y);
		obj.transform.position = topLeftCenter + 0.5f * constructible.GetLogicalSize() * tileSize - Vector2.one * 0.5f * tileSize;
	}

	public void AddResource(ResourceType type, int amount)
	{
		if (amount <= 0)
			return;

		resources[type] += amount;
	}

	public bool CanUseResource(ResourceType type, int amount)
	{
		return GetResourceAmount(type) >= amount;
	}

	public void UseResource(ResourceType type, int amount)
	{
		if (!CanUseResource(type, amount))
			return;

		resources[type] -= amount;
	}

	public int GetResourceAmount(ResourceType type)
	{
		return resources[type];
	}
}
