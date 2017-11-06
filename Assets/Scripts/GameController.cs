using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	public int initialCurrency;
	public float tileSize;
	public Island island;

	private void Awake()
	{
		Debug.Assert(Instance == null);
		Instance = this;
	}

	private void Start()
	{
		island.GenerateRandom();
	}
}
