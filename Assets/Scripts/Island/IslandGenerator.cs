using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
	public int randomSeed;
	
	public int mapWidth;
	public int mapHeight;
	public int borderSize;

	[Range(0.0f, 1.0f)]
	public float fillChance;
	public int smoothingIterations;
	public int cellularThreshold;

	public int wallSizeThreshold;
	public int roomSizeThreshold;

	private System.Random rnd;
	private IslandData currentIsland;

	public enum TileType
	{
		Floor,
		Wall
	}

	public class IslandData
	{
		public TileType[,] tiles;

		public IslandData(int w, int h)
		{
			tiles = new TileType[w, h];
		}
	}

	//[Serializable]
	//public class IntRange
	//{
	//	public int min;
	//	public int max;

	//	public IntRange(int min, int max)
	//	{
	//		this.min = min;
	//		this.max = max;
	//	}

	//	public int Random(System.Random rnd)
	//	{
	//		return rnd.Next(min, max);
	//	}
	//}

	//[Serializable]
	//public class CircleRange
	//{
	//	public Point center;
	//	public int range;

	//	public Point Random(System.Random rnd, int maxWidth, int maxHeight)
	//	{
	//		var left = Mathf.Clamp(center.x - range, 0, maxWidth);
	//		var right = Mathf.Clamp(center.x + range, 0, maxWidth);
	//		var bottom = Mathf.Clamp(center.y - range, 0, maxHeight);
	//		var top = Mathf.Clamp(center.y + range, 0, maxHeight);
	//		IntRange xr = new IntRange(left, right);
	//		IntRange yr = new IntRange(bottom, top);

	//		Point pt = null;
	//		while (pt == null || !IsInRange(pt))
	//		{
	//			pt = new Point(xr.Random(rnd), yr.Random(rnd));
	//		}

	//		return pt;
	//	}

	//	public bool IsInRange(Point pt)
	//	{
	//		return Point.ManhattanDistance(pt, center) <= range;
	//	}
	//}

	[Serializable]
	public class Point
	{
		public int x, y;

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static int ManhattanDistance(Point pt1, Point pt2)
		{
			return Mathf.Abs(pt2.x - pt1.x) + Mathf.Abs(pt2.y - pt1.y);
		}
	}

	public IslandData GenerateDungeon()
	{
		rnd = new System.Random(randomSeed);

		currentIsland = new IslandData(mapWidth, mapHeight);

		GenerateTiles();

		return currentIsland;
	}

	void GenerateTiles()
	{
		FillRandomly();
		for (int i = 0; i < smoothingIterations; i++)
			Smooth();

		//FillSmallRegions();
	}

	void FillRandomly()
	{
		for (int x = 0; x < mapWidth; ++x)
		{
			for (int y = 0; y < mapHeight; ++y)
			{
				if (x < borderSize || x > mapWidth - borderSize - 1 || y < borderSize || y > mapHeight - borderSize - 1)
					currentIsland.tiles[x, y] = TileType.Wall;
				else
					currentIsland.tiles[x, y] = (rnd.NextDouble() < fillChance) ? TileType.Floor : TileType.Wall;
			}
		}
	}

	void Smooth()
	{
		for (int x = 0; x < mapWidth; ++x)
		{
			for (int y = 0; y < mapHeight; ++y)
			{
				int wallCount = CountSurroundingWalls(x, y);

				if (wallCount > cellularThreshold)
					currentIsland.tiles[x, y] = TileType.Wall;
				else if (wallCount < cellularThreshold)
					currentIsland.tiles[x, y] = TileType.Floor;
			}
		}
	}

	int CountSurroundingWalls(int cx, int cy)
	{
		int wallsCount = 0;

		for (int x = cx - 1; x <= cx + 1; ++x)
		{
			for (int y = cy - 1; y <= cy + 1; ++y)
			{
				if (x == cx && y == cy)
					continue;

				if (!IsInMapRange(x, y) || currentIsland.tiles[x, y] == TileType.Wall)
					wallsCount++;
			}
		}

		return wallsCount;
	}

	bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
	}

	void FillSmallRegions()
	{

	}

	List<List<Point>> GetRegions(TileType tileType)
	{
		List<List<Point>> regions = new List<List<Point>>();
		bool[,] flooded = new bool[mapWidth, mapHeight];

		for (int x = 0; x < mapWidth; ++x)
		{
			for (int y = 0; y < mapHeight; ++y)
			{
				if (!flooded[x, y] && currentIsland.tiles[x, y] == tileType)
				{
					List<Point> newRegion = GetRegionTiles(flooded, x, y);
					regions.Add(newRegion);

					foreach (Point tile in newRegion)
					{
						flooded[tile.x, tile.y] = true;
					}
				}
			}
		}

		return regions;
	}

	List<Point> GetRegionTiles(bool[,] flooded, int startX, int startY)
	{
		List<Point> tiles = new List<Point>();
		var tileType = currentIsland.tiles[startX, startY];

		Queue<Point> queue = new Queue<Point>();
		queue.Enqueue(new Point(startX, startY));
		flooded[startX, startY] = true;

		while (queue.Count > 0)
		{
			Point tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.x - 1; x <= tile.x + 1; ++x)
			{
				for (int y = tile.y - 1; y <= tile.y + 1; ++y)
				{
					if (IsInMapRange(x, y) && (y == tile.y || x == tile.x))
					{
						if (!flooded[x, y] && currentIsland.tiles[x, y] == tileType)
						{
							flooded[x, y] = true;
							queue.Enqueue(new Point(x, y));
						}
					}
				}
			}
		}

		return tiles;
	}
}
