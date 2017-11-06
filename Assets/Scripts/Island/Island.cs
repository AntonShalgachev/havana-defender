using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
	[SerializeField]
	private int gridCols;
	[SerializeField]
	private int gridRows;

	[SerializeField]
	private int smoothingThreshold;
	[SerializeField]
	private int smoothingIterations;

	public GameObject wallTile;
	public float tileSize;
	public GameObject tilesObject;

	private bool[,] currentGrid;

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

				var pos = GetTileCenter(i, j);
				var tile = Instantiate(wallTile, pos, Quaternion.identity, tilesObject.transform);

				var tileWidth = tile.GetComponent<SpriteRenderer>().bounds.size.x;
				tile.transform.localScale = Vector2.Scale(tile.transform.localScale, Vector2.one * tileSize / tileWidth);
			}
		}
	}

	Vector2 GetTileCenter(int x, int y)
	{
		var width = currentGrid.GetLength(0);
		var height = currentGrid.GetLength(1);
		return new Vector2((x - 0.5f * width) * tileSize, (y - 0.5f * height) * tileSize);
	}
}
