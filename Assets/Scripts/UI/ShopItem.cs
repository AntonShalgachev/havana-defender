using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
	public int price;
	public BuildingType type;

	public Button button;
	public UIResource currency;

	private void Start()
	{
		currency.SetValue(price);
		button.onClick.AddListener(OnBuildClicked);
	}

	void OnBuildClicked()
	{
		GameController.Instance.OnBuildButtonClick(type);
	}
}
