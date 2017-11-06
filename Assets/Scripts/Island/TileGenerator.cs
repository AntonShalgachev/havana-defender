using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
	public GameObject emptyTile;
	public GameObject wallTile;
	public float tileSize;
	
	public GameObject tilesObject;

	IslandGenerator.IslandData currentDungeon;

	public void GenerateIsland()
	{
		var dungeonGenerator = GetComponent<IslandGenerator>();
		currentDungeon = dungeonGenerator.GenerateDungeon();
		
		GenerateTiles();
	}

	void GenerateTiles()
	{
		var map = currentDungeon.tiles;
		var width = map.GetLength(0);
		var height = map.GetLength(1);

		foreach (Transform child in tilesObject.transform)
			Destroy(child.gameObject);

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				var prefab = emptyTile;
				if (map[i, j] == IslandGenerator.TileType.Wall)
					prefab = wallTile;
				
				var pos = GetTileCenter(i, j);

				var tile = Instantiate(prefab, pos, Quaternion.identity, tilesObject.transform);
				var tileWidth = tile.GetComponent<SpriteRenderer>().bounds.size.x;
				tile.transform.localScale = Vector2.Scale(tile.transform.localScale, Vector2.one * tileSize / tileWidth);
			}
		}
	}

	Vector2 GetTileCenter(int x, int y)
	{
		var width = currentDungeon.tiles.GetLength(0);
		var height = currentDungeon.tiles.GetLength(1);
		return new Vector2((x - 0.5f * width) * tileSize, (y - 0.5f * height) * tileSize);
	}
}
