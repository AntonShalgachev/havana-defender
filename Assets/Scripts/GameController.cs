using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
	public static GameController Instance;

	public float tileSize;
	public Island island;

	public GameObject cannonPrefab;
	public GameObject warehousePrefab;
	public GameObject barracksPrefab;

	public UIResource uiGoldResource;
	public UIResource uiAmmoResource;
	public UIResource uiSoldiersResource;

	public GameObject shipPrefab;
	public GameObject shipsObject;
	public GameObject[] shipSpawns;

	public float wavesInterval;
	public float shipSpawnInterval;
	public int[] shipsPerWave;
	public int maxShipsBoarding;

	public StatusBar statusBar;
	public GameObject gameOverPlate;

	GameObject currentBuilding = null;
	int currentPrice;
	
	float shipSpawnDelay;
	int shipsLeftToDestroy;
	int shipsLeftToSpawn;
	int wave = -1;
	bool waveEnded = true;
	float timeBeforeNextWave;
	int shipsBoarded = 0;
	bool gameOver = false;

	private void Awake()
	{
		Debug.Assert(Instance == null);
		Instance = this;
		gameOverPlate.SetActive(false);
	}

	private void Start()
	{
		island.InitRandom();

		StartWaveIn(wavesInterval);
	}

	private void Update()
	{
		if (gameOver)
			return;

		uiGoldResource.SetValue(island.GetResourceAmount(Island.ResourceType.Gold));
		uiAmmoResource.SetValue(island.GetResourceAmount(Island.ResourceType.Ammo));
		uiSoldiersResource.SetValue(island.GetResourceAmount(Island.ResourceType.Soldier));

		if (currentBuilding)
		{
			var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			currentBuilding.transform.position = new Vector3(pos.x, pos.y, currentBuilding.transform.position.z);

			island.HighlightCellsForConstructible(currentBuilding);
		}

		if (Input.GetMouseButtonDown(0) && currentBuilding != null)
		{
			if (island.CanBuildConstructible(currentBuilding) && island.CanUseResource(Island.ResourceType.Gold, currentPrice))
			{
				island.BuildConstructible(currentBuilding);
				island.UseResource(Island.ResourceType.Gold, currentPrice);
			}
			else
			{
				DestroyObject(currentBuilding);
			}

			currentBuilding = null;
			island.UnhighlightAll();
		}

		if (Input.GetButtonDown("Cancel") && currentBuilding != null)
		{
			DestroyObject(currentBuilding);

			currentBuilding = null;
			island.UnhighlightAll();
		}

		timeBeforeNextWave -= Time.deltaTime;
		statusBar.SetWave(wave);
		statusBar.SetNextWaveIn(timeBeforeNextWave);
		statusBar.SetShipsBoarded(shipsBoarded, maxShipsBoarding);

		shipSpawnDelay -= Time.deltaTime;

		if (!waveEnded && shipSpawnDelay < 0.0f && shipsLeftToSpawn > 0)
		{
			SpawnShip();

			shipSpawnDelay = shipSpawnInterval;
			shipsLeftToDestroy++;
			shipsLeftToSpawn--;
		}
	}

	public Island GetIsland()
	{
		return island;
	}

	GameObject GetPrefab(BuildingType type)
	{
		switch (type)
		{
			case BuildingType.Cannon:
				return cannonPrefab;
			case BuildingType.Warehouse:
				return warehousePrefab;
			case BuildingType.Barracks:
				return barracksPrefab;
		}

		Debug.Assert(false);
		return null;
	}

	public void OnBuildButtonClick(BuildingType type, int price)
	{
		if (currentBuilding != null)
			return;

		if (!island.CanUseResource(Island.ResourceType.Gold, price))
			return;
		
		currentBuilding = Instantiate(GetPrefab(type), Vector2.zero, Quaternion.identity);
		currentPrice = price;
	}

	public void CollectReward(int reward)
	{
		island.AddResource(Island.ResourceType.Gold, reward);
	}

	public void OnShipDestroyed(GameObject ship)
	{
		shipsLeftToDestroy--;
		if (shipsLeftToDestroy <= 0)
			EndWave();
	}

	public void OnShipBoardedFort(GameObject ship)
	{
		shipsBoarded++;
		if (shipsBoarded > maxShipsBoarding)
			EndGame();
	}

	int GetWaveShips()
	{
		int size = shipsPerWave.Length;
		if (wave >= size)
			return shipsPerWave[size - 1];

		return shipsPerWave[wave];
	}

	void StartWaveIn(float delay)
	{
		Debug.LogFormat("Starting new wave in {0}", delay);

		timeBeforeNextWave = delay;
		Invoke("StartWave", delay);
	}

	void StartWave()
	{
		Debug.Log("Starting new wave");

		wave++;
		waveEnded = false;
		shipSpawnDelay = shipSpawnInterval;
		shipsLeftToSpawn = GetWaveShips();
		shipsBoarded = 0;

		statusBar.WaveStarted();
	}

	void EndWave()
	{
		Debug.Log("Ending wave");

		waveEnded = true;
		StartWaveIn(wavesInterval);

		statusBar.WaveEnded();
	}

	void SpawnShip()
	{
		Debug.Log("Spawning ship");

		var index = Random.Range(0, shipSpawns.Length);
		var spawn = shipSpawns[index];

		Instantiate(shipPrefab, spawn.transform.position, Quaternion.identity, shipsObject.transform);
	}

	public float GetTimeBeforeWave()
	{
		return timeBeforeNextWave;
	}

	public bool IsWaveEnded()
	{
		return waveEnded;
	}

	private void OnDestroy()
	{
		Debug.Assert(Instance == this);
		Instance = null;
	}

	void EndGame()
	{
		gameOverPlate.SetActive(true);
		gameOver = true;
		Debug.Log("Game ended");
	}
}
