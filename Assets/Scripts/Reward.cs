using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
	public int reward;

	private void OnDestroy()
	{
		if (GameController.Instance)
			GameController.Instance.CollectReward(reward);
	}
}
