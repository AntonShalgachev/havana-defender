using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public int initialCurrency;
	public Island island;

	private void Start()
	{
		island.GenerateRandom();
	}
}
