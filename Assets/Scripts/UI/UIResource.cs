using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResource : MonoBehaviour
{
	public Text text;

	public void SetValue(int val)
	{
		text.text = val.ToString();
	}
}
