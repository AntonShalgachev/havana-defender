using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
	public int reward;

	private void OnDestroy()
	{
		GameController.Instance.CollectReward(reward);
	}
}
