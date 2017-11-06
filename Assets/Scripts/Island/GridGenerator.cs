using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class GridGenerator
{
	private class FloodFiller
	{
		public struct Pos
		{
			public int row;
			public int col;

			public Pos(int row, int col)
			{
				this.row = row;
				this.col = col;
			}
		}

		private int rows;
		private int cols;
		private bool[,] grid;
		private bool refValue;
		private bool[,] filled;

		public FloodFiller(bool[,] grid, bool refValue)
		{
			rows = grid.GetLength(0);
			cols = grid.GetLength(1);

			this.grid = grid;
			this.refValue = refValue;

			filled = new bool[rows, cols];
			for (var row = 0; row < rows; row++)
				for (var col = 0; col < cols; col++)
					filled[row, col] = false;
		}

		public List<Pos> FloodFill(int startRow, int startCol)
		{
			Debug.Assert(grid[startRow, startCol] == refValue);

			List<Pos> result = new List<Pos>();

			Queue<Pos> queue = new Queue<Pos>();
			queue.Enqueue(new Pos(startRow, startCol));
			filled[startRow, startCol] = true;

			while(queue.Count != 0)
			{
				Pos pos = queue.Dequeue();
				result.Add(pos);

				var offsets = new int[][] { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } };
				foreach(var offset in offsets)
				{
					var row = pos.row + offset[0];
					var col = pos.col + offset[1];

					if (IsInGrid(row, col) && grid[row, col] == refValue && !filled[row, col])
					{
						queue.Enqueue(new Pos(row, col));
						filled[row, col] = true;
					}
				}
			}

			return result;
		}

		public bool IsFilled(int row, int col)
		{
			return filled[row, col];
		}

		bool IsInGrid(int row, int col)
		{
			return row >= 0 && row < rows && col >= 0 && col < cols;
		}
	}

	private int rows;
	private int cols;

	public bool[,] grid;

	public GridGenerator(int rows, int cols)
	{
		this.rows = rows;
		this.cols = cols;
	}

	public void GenerateRandom()
	{
		grid = new bool[rows, cols];

		for (var row = 0; row < rows; row++)
		{
			for (var col = 0; col < cols; col++)
			{
				var x = 2.0f * col / cols - 1.0f;
				var y = 2.0f * row / rows - 1.0f;
				var prob = GetProbability(x, y);
				grid[row, col] = Random.Range(0, 100) < prob;
			}
		}
	}

	public void Smooth(int iterations, int threshold)
	{
		for (var it = 0; it < iterations; it++)
			SmoothIteration(threshold);
	}

	public void Smooth(int threshold)
	{
		var iterationNeeded = true;
		while(iterationNeeded)
			iterationNeeded = SmoothIteration(threshold);
	}

	public void GenerateSmoothRegion(int iterations, int threshold)
	{
		GenerateRandom();

		if (iterations >= 0)
			Smooth(iterations, threshold);
		else
			Smooth(threshold);

		RemoveAllIslands();

		CreateBorder();
	}

	public void CreateBorder()
	{
		for (var row = 0; row < grid.GetLength(0); row++)
		{
			grid[row, 0] = false;
			grid[row, cols - 1] = false;
		}
		for (var col = 0; col < grid.GetLength(1); col++)
		{
			grid[0, col] = false;
			grid[rows - 1, col] = false;
		}
	}

	public bool RemoveAllIslands()
	{
		var res1 = RemoveIslands(true);
		var res2 = RemoveIslands(false);

		return res1 || res2;
	}

	public bool RemoveIslands(bool refValue)
	{
		FloodFiller floodFiller = new FloodFiller(grid, refValue);

		List<FloodFiller.Pos> prevRegion = null;
		bool filled = false;

		for (var row = 0; row < rows; row++)
		{
			for (var col = 0; col < cols; col++)
			{
				if (grid[row, col] == refValue && !floodFiller.IsFilled(row, col))
				{
					var newRegion = floodFiller.FloodFill(row, col);
					if (prevRegion == null)
					{
						prevRegion = newRegion;
						continue;
					}

					var fillRegion = newRegion;
					if (prevRegion.Count < newRegion.Count)
					{
						fillRegion = prevRegion;
						prevRegion = newRegion;
					}

					foreach (var pos in fillRegion)
					{
						grid[pos.row, pos.col] = !refValue;
						filled = true;
					}
				}
			}
		}

		return filled;
	}

	int GetProbability(float x, float y)
	{
		var r = Mathf.Sqrt(x * x + y * y);
		var p = 2.0f * Mathf.Pow(1.0f - r, 2.0f);
		if (r > 1.0f)
			p = 0.0f;

		return Mathf.Clamp((int)(100 * p), 0, 100);
	}

	bool SmoothIteration(int threshold)
	{
		bool gridModified = false;
		var gridCopy = new bool[rows, cols];
		for (int row = 0; row < rows; ++row)
		{
			for (int col = 0; col < cols; ++col)
			{
				int surroundingCellsCount = CountSurroundingCells(row, col);

				if (surroundingCellsCount > threshold)
					gridCopy[row, col] = true;
				else if (surroundingCellsCount < threshold)
					gridCopy[row, col] = false;
				else
					gridCopy[row, col] = grid[row, col];

				if (gridCopy[row, col] != grid[row, col])
					gridModified = true;
			}
		}

		grid = gridCopy;

		return gridModified;
	}

	int CountSurroundingCells(int gridRow, int gridCol)
	{
		int surroundingCells = 0;

		for (var drow = -1; drow <= 1; drow++)
		{
			for (var dcol = -1; dcol <= 1; dcol++)
			{
				var row = gridRow + drow;
				var col = gridCol + dcol;
				if (IsInGrid(row, col) && (drow != 0 || dcol != 0) && grid[row, col])
					surroundingCells++;
			}
		}

		return surroundingCells;
	}

	bool IsInGrid(int row, int col)
	{
		return row >= 0 && row < rows && col >= 0 && col < cols;
	}
}
