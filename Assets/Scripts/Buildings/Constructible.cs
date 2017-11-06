using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructible : MonoBehaviour
{
	public int width;
	public int height;
	public float visualScale;
	public BuildingType type;

	private Island island;

	private void Start()
	{
		var sprite = GetComponent<SpriteRenderer>();
		var spriteSize = sprite.bounds.size.x;
		var tileSize = GameController.Instance.tileSize;

		var targetSize = new Vector2(width, height) * visualScale * tileSize;

		transform.localScale = Vector2.Scale(transform.localScale, targetSize) / spriteSize;
	}

	public void AttachToIsland(Island island)
	{
		this.island = island;
	}

	public Island GetIsland()
	{
		return island;
	}

	public Vector2 GetLogicalSize()
	{
		return new Vector2(width, height);
	}
}
