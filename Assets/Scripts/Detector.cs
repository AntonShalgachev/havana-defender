using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
	public string targetLayer;

	List<GameObject> inRange = new List<GameObject>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("OnTriggerEnter2D");

		var obj = collision.gameObject;
		if (obj.layer == LayerMask.NameToLayer(targetLayer))
			inRange.Add(obj);
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		var obj = collider.gameObject;
		if (obj.layer == LayerMask.NameToLayer(targetLayer))
			inRange.Remove(obj);
	}

	public List<GameObject> GetInRange()
	{
		return inRange;
	}

	public GameObject GetClosest()
	{
		GameObject closestObj = null;

		foreach (var obj in inRange)
		{
			if (obj == null)
				continue;

			var dir = obj.transform.position - transform.position;
			var dist = dir.magnitude;

			if (closestObj == null)
			{
				closestObj = obj;
			}
			else
			{
				var closestDist = (transform.position - closestObj.transform.position).magnitude;
				if (dist < closestDist)
					closestObj = obj;
			}
		}

		return closestObj;
	}
}
