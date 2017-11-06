using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructible : MonoBehaviour
{
	public int width;
	public int height;

	private void Start()
	{
		var sprite = GetComponent<SpriteRenderer>();
		var spriteSize = sprite.bounds.size.x;
		var tileSize = GameController.Instance.tileSize;

		var targetSize = new Vector2(width * tileSize, height * tileSize);

		transform.localScale = Vector2.Scale(transform.localScale, targetSize) / spriteSize;
	}
}
