using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
	public Text currentWave;
	public Text nextWaveIn;
	public Text shipsBoarded;

	public void SetWave(int wave)
	{
		currentWave.text = string.Format("Wave {0}", wave + 1);
	}

	public void SetNextWaveIn(float delay)
	{
		nextWaveIn.text = string.Format("Prepare your cannons! Next wave in {0}s", Mathf.FloorToInt(delay));
	}

	public void SetShipsBoarded(int ships, int max)
	{
		shipsBoarded.text = string.Format("Ships boarded your fort: {0}/{1}", ships, max);
	}

	public void WaveStarted()
	{
		nextWaveIn.gameObject.SetActive(false);
		shipsBoarded.gameObject.SetActive(true);
	}

	public void WaveEnded()
	{
		nextWaveIn.gameObject.SetActive(true);
		shipsBoarded.gameObject.SetActive(false);
	}
}
